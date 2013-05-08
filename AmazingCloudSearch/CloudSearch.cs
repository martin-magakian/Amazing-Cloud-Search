using System;
using System.Collections.Generic;
using AmazingCloudSearch.Builder;
using AmazingCloudSearch.Contract;
using AmazingCloudSearch.Contract.Result;
using AmazingCloudSearch.Enum;
using AmazingCloudSearch.Helper;
using AmazingCloudSearch.Query;
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

    public class CloudSearch<TDocument> where TDocument : ISearchDocument, new()
    {
        readonly string _documentUri;
        readonly string _searchUri;
        readonly ActionBuilder<TDocument> _actionBuilder;
        readonly WebHelper _webHelper;
        readonly ICloudSearchSettings _cloudSearchSettings;
        readonly IQueryBuilder<TDocument> _queryBuilder;
        readonly HitFeeder<TDocument> _hitFeeder;
        readonly FacetBuilder _facetBuilder;

        public CloudSearch(ICloudSearchSettings cloudSearchSettings, IQueryBuilder<TDocument> queryBuilder)
        {
            _cloudSearchSettings = cloudSearchSettings;
            _queryBuilder = queryBuilder;
        }

        public CloudSearch(ICloudSearchSettings cloudSearchSettings)
        {
            _cloudSearchSettings = cloudSearchSettings;
            _searchUri = string.Format("http://search-{0}/{1}/search", _cloudSearchSettings.CloudSearchId, _cloudSearchSettings.ApiVersion);
            _documentUri = string.Format("http://doc-{0}/{1}/documents/batch", _cloudSearchSettings.CloudSearchId, _cloudSearchSettings.ApiVersion);
            _actionBuilder = new ActionBuilder<TDocument>();
            _queryBuilder = new QueryBuilder<TDocument>(_searchUri);
            _webHelper = new WebHelper();
            _hitFeeder = new HitFeeder<TDocument>();
            _facetBuilder = new FacetBuilder();
        }

        public CloudSearch(string awsCloudSearchId, string apiVersion) : this (new CloudSearchSettings(){ApiVersion = apiVersion, CloudSearchId = awsCloudSearchId})
        {                                    
        }

        TResult Add<TResult>(IEnumerable<TDocument> liToAdd) where TResult : BasicResult, new()
        {
            var liAction = new List<BasicDocumentAction>();

            foreach (var toAdd in liToAdd)
            {
                var action = _actionBuilder.BuildAction(toAdd, ActionType.ADD);
                liAction.Add(action);
            }

            return PerformDocumentAction<TResult>(liAction);
        }

        TResult Add<TResult>(TDocument toAdd) where TResult : BasicResult, new()
        {
            var action = _actionBuilder.BuildAction(toAdd, ActionType.ADD);

            return PerformDocumentAction<TResult>(action);
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

        public DeleteResult Delete(ISearchDocument toDelete)
        {
            var action = _actionBuilder.BuildDeleteAction(new SearchDocument {id = toDelete.id}, ActionType.DELETE);

            return PerformDocumentAction<DeleteResult>(action);
        }

        public SearchResult<TDocument> Search(SearchQuery<TDocument> query)
        {
            try
            {
                return SearchWithException(query);
            }
            catch (Exception ex)
            {
                return new SearchResult<TDocument> {error = "An error occured " + ex.Message, IsError = true};
            }
        }

        SearchResult<TDocument> SearchWithException(SearchQuery<TDocument> query)
        {
            var searchUrlRequest = _queryBuilder.BuildSearchQuery(query);

            var jsonResult = _webHelper.GetRequest(searchUrlRequest);

            if (jsonResult.IsError)
            {
                return new SearchResult<TDocument> {error = jsonResult.exeption, IsError = true};
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
                return new TResult {IsError = true, status = "error", errors = new List<Error> {new Error {message = jsonResult.exeption}}};
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
            var listAction = new List<BasicDocumentAction> {basicDocumentAction};

            return PerformDocumentAction<TResult>(listAction);
        }
    }
}
