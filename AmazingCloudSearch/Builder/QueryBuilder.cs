using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AmazingCloudSearch.Contract;
using AmazingCloudSearch.Query;
using AmazingCloudSearch.Query.Boolean;
using AmazingCloudSearch.Query.Facets;

namespace AmazingCloudSearch.Builder
{
    /*     
     * The rules when building the URL string it that ONLY function who write
     * in the string add the & at the end of it.    
     */
    public interface IQueryBuilder<T> where T : ISearchDocument, new() {
        string BuildSearchQuery(SearchQuery<T> query);
        string BuildFromPublicSearchQuery(string publicSearchQueryString, SearchQuery<T> query);
        string BuildPublicSearchQuery(SearchQuery<T> query);
    }

    public class QueryBuilder<T> : IQueryBuilder<T> where T : ISearchDocument, new()
    {
        readonly string _searchUri;

        public QueryBuilder() {}

        public QueryBuilder(string searchUri)
        {
            _searchUri = searchUri;
        }

        public string BuildSearchQuery(SearchQuery<T> query)
        {
            if (!string.IsNullOrEmpty(query.PublicSearchQueryString))
            {
                return BuildFromPublicSearchQuery(query.PublicSearchQueryString, query);
            }

            var url = new StringBuilder(_searchUri);
            url.Append("?");

            if (query.Keyword != null)
            {
                FeedKeyword(query.Keyword, url);
            }

            if (query.BooleanQuery != null)
            {
                FeedBooleanCritera(query.BooleanQuery, url);
            }

            if (query.Facets != null)
            {
                FeedFacet(query.Facets, url);
            }

            if (query.Fields != null)
            {
                FeedReturnFields(query.Fields, url);
            }

            if (query.Size != null)
            {
                FeedMaxResults(query.Size, url);
            }

            if (query.Start != null)
            {
                FeedStartResultFrom(query.Start, url);
            }

            return url.ToString();
        }

        public string BuildFromPublicSearchQuery(string publicSearchQueryString, SearchQuery<T> query)
        {
            var url = new StringBuilder(_searchUri);
            url.Append("?");

            url.Append(publicSearchQueryString);

            FeedFacet(query.Facets, url);

            FeedReturnFields(query.Fields, url);

            FeedMaxResults(query.Size, url);

            FeedStartResultFrom(query.Start, url);

            return url.ToString();
        }

        public string BuildPublicSearchQuery(SearchQuery<T> query)
        {
            var url = new StringBuilder();

            FeedKeyword(query.Keyword, url);

            FeedBooleanCritera(query.BooleanQuery, url);

            FeedFacet(query.Facets, url);

            return url.ToString();
        }

        public void FeedKeyword(string keyword, StringBuilder url)
        {
            if (!string.IsNullOrEmpty(keyword))
            {
                url.Append("q=");
                url.Append(Uri.EscapeDataString(keyword));
            }
        }

        public void FeedBooleanCritera(BooleanQuery booleanQuery, StringBuilder url)
        {
            if (booleanQuery.Conditions == null || booleanQuery.Conditions.Count == 0)
            {
                return;
            }

            var hasParameters = (url.Length > 0);

            var andConditions = new StringBuilder();
            var orConditions = new StringBuilder();
            var listOrConditions = new List<string>();

            foreach (var condition in booleanQuery.Conditions)
            {
                if (condition.IsOrCondition())
                {
                    listOrConditions.Add(condition.GetConditionParam());
                }
                else
                {
                    andConditions.Append(condition.GetConditionParam());
                    andConditions.Append("+");
                }
            }

            var booleanConditions = new List<string>();

            if (andConditions.Length > 0)
            {
                andConditions.Remove(andConditions.Length - 1, 1);
                booleanConditions.Add("and+" + andConditions);
            }

            if (orConditions.Length > 0)
            {
                orConditions.Remove(orConditions.Length - 1, 1);
                listOrConditions.Add(orConditions.ToString());
            }

            if (listOrConditions.Count == 1)
            {
                booleanConditions.Add("or+" + listOrConditions[0]);
            }
            else if (listOrConditions.Count > 1)
            {
                orConditions.Clear();
                orConditions.Append("and");

                foreach (string listOrCondintion in listOrConditions)
                {
                    orConditions.Append("+(or+");
                    orConditions.Append(listOrCondintion);
                    orConditions.Append(")");
                }

                booleanConditions.Add(orConditions.ToString());
            }

            if (hasParameters)
            {
                url.Append("&");
            }

            url.Append("bq=");

            string postpendBooleanCondition = null;
            int count = 0;
            foreach (string booleanCondition in booleanConditions)
            {
                if (count > 0)
                {
                    url.Append("+");
                }

                url.Append("(");
                url.Append(booleanCondition);
                postpendBooleanCondition += ")";

                count++;
            }

            url.Append(postpendBooleanCondition);
        }

        public void FeedStartResultFrom(int? start, StringBuilder url)
        {
            if (start != null)
            {
                url.Append("&");
                url.Append("start=");
                url.Append(start);
            }
        }

        public void FeedMaxResults(int? size, StringBuilder url)
        {
            if (size != null)
            {
                url.Append("&");
                url.Append("size=");
                url.Append(size);
            }
        }

        public void FeedFacet(List<Facet> facets, StringBuilder url)
        {
            FeedFacetList(facets, url);

            FeedFacetConstraints(facets, url);
        }

        public void FeedFacetList(List<Facet> facets, StringBuilder url)
        {
            if (facets == null || facets.Count == 0)
            {
                return;
            }

            bool hasParameters = (url.Length > 0);

            if (hasParameters)
            {
                url.Append("&");
            }

            url.Append("facet=");

            Facet lastItem = facets.Last();
            foreach (var facet in facets)
            {
                url.Append(facet.Name);

                if (!ReferenceEquals(lastItem, facet))
                {
                    url.Append(",");
                }
            }
        }

        public void FeedFacetConstraints(List<Facet> facets, StringBuilder url)
        {
            if (facets == null || facets.Count == 0)
            {
                return;
            }

            foreach (var facet in facets)
            {
                FeedFacet(facet, url);
            }
        }

        public void FeedFacet(Facet facet, StringBuilder url)
        {
            if (string.IsNullOrEmpty(facet.Name))
            {
                return;
            }

            if (facet.TopResult != null)
            {
                url.Append("&");
                url.Append("facet-");
                url.Append(facet.Name);
                url.Append("-top-n=");
                url.Append(facet.TopResult);
            }

            if (facet.FacetContraint != null)
            {
                var param = facet.FacetContraint.GetRequestParam();
                if (param != null)
                {
                    url.Append("&");
                    url.Append("facet-");
                    url.Append(facet.Name);
                    url.Append("-constraints=");
                    url.Append(param);
                }
            }
        }

        public void FeedReturnFields(List<string> fields, StringBuilder url)
        {
            if (fields == null || fields.Count == 0)
            {
                return;
            }

            bool hasParameters = (url.Length > 0);

            if (hasParameters)
            {
                url.Append("&");
            }

            url.Append("return-fields=");

            foreach (var field in fields)
            {
                url.Append(field);
                url.Append(",");
            }

            if (url.Length > 0)
            {
                url.Remove(url.Length - 1, 1);
            }
        }
    }
}
