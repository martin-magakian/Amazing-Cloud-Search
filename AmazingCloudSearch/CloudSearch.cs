using System;
using System.Collections.Generic;
using System.Linq;
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

    public interface IMultiTenantCloudSearchSettings : ICloudSearchSettings
    {
        string TenantParameterName { get; set; }
        string Tenant { get; set; }
    }

    public class CloudSearchSettings : ICloudSearchSettings
    {
        public string CloudSearchId { get; set; }
        public string ApiVersion { get; set; }
    }

    public class MultiTenantCloudSearchSettings : CloudSearchSettings, IMultiTenantCloudSearchSettings
    {
        public string TenantParameterName { get; set; }
        public string Tenant { get; set; }
    }

    public interface ICloudSearch<TDocument> where TDocument : ICloudSearchDocument, new()
    {
        AddResult Add(List<TDocument> toAdd);
        AddResult Add(TDocument toAdd);
        UpdateResult Update(TDocument toUpdate);
        DeleteResult Delete(ICloudSearchDocument toDelete);
        DeleteResult Delete(List<ICloudSearchDocument> toDelete);
        SearchResult<TDocument> Search(SearchQuery<TDocument> query);
    }

    public class MultiTenantCloudSearch<TDocument> : CloudSearch<TDocument> where TDocument : ICloudSearchDocument, new()
    {
        private readonly IMultiTenantCloudSearchSettings _multiTenantCloudSearchSettings;

        public MultiTenantCloudSearch(IMultiTenantCloudSearchSettings multiTenantCloudSearchSettings, IQueryBuilder<TDocument> queryBuilder) : base(multiTenantCloudSearchSettings, queryBuilder)
        {
            _multiTenantCloudSearchSettings = multiTenantCloudSearchSettings;
        }

        public MultiTenantCloudSearch(IMultiTenantCloudSearchSettings multiTenantCloudSearchSettings)
            : base(multiTenantCloudSearchSettings)
        {
            _multiTenantCloudSearchSettings = multiTenantCloudSearchSettings;
        }

        public MultiTenantCloudSearch(string awsCloudSearchId, string apiVersion) : base(awsCloudSearchId, apiVersion)
        {
        }

        public override SearchResult<TDocument> Search(SearchQuery<TDocument> query)
        {
            try
            {                
                var tenantCondition = CreateTenantBooleanCondition();
                AddTenantBooleanConditionToQuery(tenantCondition, query);                
                return SearchWithException(query);
            }
            catch (Exception ex)
            {
                return new SearchResult<TDocument> { error = "An error occured " + ex.Message, IsError = true };
            }
        }

        public SearchQuery<TDocument> AddTenantBooleanConditionToQuery(StringBooleanCondition tenantCondition, SearchQuery<TDocument> query)
        {
            query.BooleanQuery.Conditions.Add(tenantCondition);
            return query;
        }

        public StringBooleanCondition CreateTenantBooleanCondition()
        {
            var returnValue = new StringBooleanCondition(_multiTenantCloudSearchSettings.TenantParameterName,
                                                         _multiTenantCloudSearchSettings.Tenant);
            return returnValue;
        }
    }

    public class CloudSearch<TDocument> : ICloudSearch<TDocument> where TDocument : ICloudSearchDocument, new()
    {
        readonly string _documentUri;
        readonly string _searchUri;
        readonly ActionBuilder<TDocument> _actionBuilder;
        readonly WebHelper _webHelper;
        readonly ICloudSearchSettings _multiTenantCloudSearchSettings;
        readonly IQueryBuilder<TDocument> _queryBuilder;
        readonly HitFeeder<TDocument> _hitFeeder;
        readonly FacetBuilder _facetBuilder;

        public CloudSearch(ICloudSearchSettings multiTenantCloudSearchSettings, IQueryBuilder<TDocument> queryBuilder)
        {
            _multiTenantCloudSearchSettings = multiTenantCloudSearchSettings;
            _queryBuilder = queryBuilder;
            
            _searchUri = string.Format("http://search-{0}/{1}/search", _multiTenantCloudSearchSettings.CloudSearchId, _multiTenantCloudSearchSettings.ApiVersion);
            _documentUri = string.Format("http://doc-{0}/{1}/documents/batch", _multiTenantCloudSearchSettings.CloudSearchId, _multiTenantCloudSearchSettings.ApiVersion);
            _actionBuilder = new ActionBuilder<TDocument>();            
            _webHelper = new WebHelper();
            _hitFeeder = new HitFeeder<TDocument>();
            _facetBuilder = new FacetBuilder();
        }

        public CloudSearch(ICloudSearchSettings multiTenantCloudSearchSettings) : this (multiTenantCloudSearchSettings, new QueryBuilder<TDocument>(multiTenantCloudSearchSettings))
        {
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

        public DeleteResult Delete(ICloudSearchDocument toDelete)
        {
            var action = _actionBuilder.BuildDeleteAction(new CloudSearchDocument {id = toDelete.id}, ActionType.DELETE);

            return PerformDocumentAction<DeleteResult>(action);
        }

        public DeleteResult Delete(List<ICloudSearchDocument> toDelete)
        {
            var action = toDelete.Select(x => _actionBuilder.BuildDeleteAction(new CloudSearchDocument { id = x.id }, ActionType.DELETE));

            return PerformDocumentAction<DeleteResult>(action.ToList());
        }

        public virtual SearchResult<TDocument> Search(SearchQuery<TDocument> query)
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

        public virtual SearchResult<TDocument> SearchWithException(SearchQuery<TDocument> query)
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
