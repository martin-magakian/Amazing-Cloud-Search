using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using AmazingCloudSearch.Builder;
using AmazingCloudSearch.Contract;
using AmazingCloudSearch.Contract.Result;
using AmazingCloudSearch.Enum;
using AmazingCloudSearch.Helper;
using AmazingCloudSearch.Query;
using AmazingCloudSearch.Query.Boolean;
using AmazingCloudSearch.Serialization;
using Newtonsoft.Json;

namespace AmazingCloudSearch
{
    public interface ICloudSearchSettings
    {
        string CloudSearchId { get; set; }
        string ApiVersion { get; set; }
    }

    public class CloudSearchSettings : ICloudSearchSettings
    {
        public string CloudSearchId { get; set; }
        public string ApiVersion { get; set; }
    }

    public interface ICloudSearch<TDocument> where TDocument : ICloudSearchDocument, new()
    {
        AddResult Add(List<TDocument> toAdd);
        AddResult Add(TDocument toAdd);
        UpdateResult Update(TDocument toUpdate);
        UpdateResult Update(List<TDocument> toUpdate);
        DeleteResult Delete(TDocument toDelete);
        DeleteResult Delete(List<TDocument> toDelete);
        SearchResult<TDocument> Search(SearchQuery<TDocument> query);
    }

    public class CloudSearch<TDocument> : ICloudSearch<TDocument> where TDocument : ICloudSearchDocument, new()
    {
        readonly string _documentUri;
        readonly string _searchUri;
        readonly ActionBuilder<TDocument> _actionBuilder;
        readonly WebHelper _webHelper;
        readonly ICloudSearchSettings _cloudSearchSettings;
        readonly IQueryBuilder<TDocument> _queryBuilder;
        readonly HitFeeder<TDocument> _hitFeeder;
        readonly FacetBuilder _facetBuilder;
        public List<IBooleanCondition> PersistanteCondition {get; set;}

        public CloudSearch(ICloudSearchSettings cloudSearchSettings)
        {
            _cloudSearchSettings = cloudSearchSettings;

            _searchUri = string.Format("http://search-{0}/{1}/search", _cloudSearchSettings.CloudSearchId, _cloudSearchSettings.ApiVersion);
            _documentUri = string.Format("http://doc-{0}/{1}/documents/batch", _cloudSearchSettings.CloudSearchId, _cloudSearchSettings.ApiVersion);
            _queryBuilder = new QueryBuilder<TDocument>(_searchUri);
            _actionBuilder = new ActionBuilder<TDocument>();
            _webHelper = new WebHelper();
            _hitFeeder = new HitFeeder<TDocument>();
            _facetBuilder = new FacetBuilder();
            PersistanteCondition = new List<IBooleanCondition>();
        }

        public CloudSearch(string awsCloudSearchId, string apiVersion)
            : this(new CloudSearchSettings() { ApiVersion = apiVersion, CloudSearchId = awsCloudSearchId })
        {
        }

        TResult Add<TResult>(IEnumerable<TDocument> liToAdd) where TResult : BasicResult, new()
        {
            var liAction = new List<BasicDocumentAction>();
            const int maxBatchSize = 5242880; // 5 MB in bytes
            var currentBatchSize = 0;

            var results = new List<TResult>();

            foreach (var toAdd in liToAdd)
            {
                var action = _actionBuilder.BuildAction(toAdd, ActionType.ADD);
                liAction.Add(action);

                currentBatchSize += liAction.GetSize();
                if (currentBatchSize > maxBatchSize)
                {
                    liAction.Remove(action);

                    // Push this batch which has reached the 5 MB max on AWS and start
                    // a new batch.
                    results.Add(PerformDocumentAction<TResult>(liAction));

                    // Reset, start of new batch
                    liAction.Clear();
                    liAction.Add(action);
                    currentBatchSize = liAction.GetSize();
                }
            }


            if (liAction.Any())
                results.Add(PerformDocumentAction<TResult>(liAction));

            var result = combineResults(liAction, results);

            return result;
        }
        
        TResult combineResults<TResult>(List<BasicDocumentAction> liAction, List<TResult> results) where TResult : BasicResult, new()
        {
            var result = Activator.CreateInstance<TResult>();            
            if (result.errors == null)
            {
                result.errors = new List<Error>();
            }
            foreach (var res in results)
            {
                if (res.errors != null && res.errors.Any())
                {
                    result.errors.AddRange(res.errors);
	                result.IsError = true;
                }
                result.adds += res.adds;
                result.deletes += res.deletes;

            }
            result.status = string.Join(", ", results.Select(x => x.status).Distinct());
            return result;
        }



        TResult Add<TResult>(TDocument toAdd) where TResult : BasicResult, new()
        {
            return Add<TResult>(new List<TDocument>() { toAdd });
        }

        public AddResult Add(List<TDocument> toAdd)
        {
            return Add<AddResult>(toAdd);
        }

        public AddResult Add(TDocument toAdd)
        {
            return Add<AddResult>(toAdd);
        }

        // update is like Add but make more sense for a developper point of view
        public UpdateResult Update(TDocument toUpdate)
        {
            return Add<UpdateResult>(toUpdate);
        }

	    public UpdateResult Update(List<TDocument> toUpdate)
	    {
		    return Add<UpdateResult>(toUpdate);
	    }

        public DeleteResult Delete(TDocument toDelete)
        {
            var action = _actionBuilder.BuildDeleteAction(new CloudSearchDocument { Id = toDelete.Id }, ActionType.DELETE);

            return PerformDocumentAction<DeleteResult>(action);
        }

        public DeleteResult Delete(List<TDocument> toDelete)
        {
            var action = toDelete.Select(x => _actionBuilder.BuildDeleteAction(new CloudSearchDocument { Id = x.Id }, ActionType.DELETE));

            return PerformDocumentAction<DeleteResult>(action.ToList());
        }

        public virtual SearchResult<TDocument> Search(SearchQuery<TDocument> query)
        {
            try
            {
                query = AddPresistantConditionsToQuery(query);
                return SearchWithException(query);
            }
            catch (Exception ex)
            {
                return new SearchResult<TDocument> { error = "An error occured " + ex.Message, IsError = true };
            }
        }

        public void AddPresistantCondition(IBooleanCondition stringBooleanCondition)
        {
            PersistanteCondition.Add(stringBooleanCondition);
        }

        public SearchQuery<TDocument> AddPresistantConditionsToQuery(SearchQuery<TDocument> query)
        {
            foreach (IBooleanCondition condition in PersistanteCondition)
            {
                query.BooleanQuery.Conditions.Add(condition);
            }
            return query;
        }

        public virtual SearchResult<TDocument> SearchWithException(SearchQuery<TDocument> query)
        {
            var searchUrlRequest = _queryBuilder.BuildSearchQuery(query);

            var jsonResult = _webHelper.GetRequest(searchUrlRequest);

            if (jsonResult.IsError)
            {
                return new SearchResult<TDocument> { error = jsonResult.exeption, IsError = true };
            }

            var jsonDynamic = JsonConvert.DeserializeObject<dynamic>(jsonResult.json);

            var hit = RemoveHit(jsonDynamic);

            var resultWithoutHit = JsonConvert.SerializeObject(jsonDynamic);

            SearchResult<TDocument> searchResult = JsonConvert.DeserializeObject<SearchResult<TDocument>>(resultWithoutHit);

            searchResult.facetsResults = _facetBuilder.BuildFacet(jsonDynamic);

            if (searchResult.error != null)
            {
                searchResult.IsError = true;
                return searchResult;
            }

            _hitFeeder.Feed(searchResult, hit);

            return searchResult;
        }

        dynamic RemoveHit(dynamic jsonDynamic)
        {
            dynamic hit = null;
            if (jsonDynamic.hits != null)
            {
                hit = jsonDynamic.hits.hit;
                jsonDynamic.hits.hit = null;
            }
            return hit;
        }


        TResult PerformDocumentAction<TResult>(List<BasicDocumentAction> liAction) where TResult : BasicResult, new()
        {
            var actionJson = JsonConvert.SerializeObject(liAction);

            var jsonResult = _webHelper.PostRequest(_documentUri, actionJson);

            if (jsonResult.IsError)
            {
                return new TResult { IsError = true, status = "error", errors = new List<Error> { new Error { message = jsonResult.exeption } } };
            }

            var result = JsonConvert.DeserializeObject<TResult>(jsonResult.json);

            if (result.status != null && result.status.Equals("error"))
            {
                result.IsError = true;
            }

            return result;
        }

        TResult PerformDocumentAction<TResult>(BasicDocumentAction basicDocumentAction) where TResult : BasicResult, new()
        {
            var listAction = new List<BasicDocumentAction> { basicDocumentAction };

            return PerformDocumentAction<TResult>(listAction);
        }


    }
}
