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
     * 
     * The rules when building the URL string it that ONLY function who write
     * in the string add the & at the end of it.
     * 
     */

    public class QueryBuilder<T> where T : SearchDocument, new()
    {
        private string _searchUri;

        public QueryBuilder(string searchUri)
        {
            _searchUri = searchUri;
        }

        public string BuildSearchQuery(SearchQuery<T> query)
        {
            var url = new StringBuilder(_searchUri);
            url.Append("?");

            FeedKeyword(query.Keyword, url);

            FeedBooleanCritera(query.BooleanQuery, url);

            FeedMaxResults(query.Size, url);

            FeedStartResultFrom(query.Start, url);

            FeedFields(query.Fields, url);

            FeedFacet(query.Facets, url);

            return url.ToString();
        }

        private void FeedBooleanCritera(BooleanQuery booleanQuery, StringBuilder url)
        {
            if(booleanQuery.Conditions == null || booleanQuery.Conditions.Count == 0)
                return;

            url.Append("bq=(and ");

            foreach (var condition in booleanQuery.Conditions)
            {
                url.Append(condition.GetCondictionParam());
                url.Append("%20");
            }
            url.Append(")&");
        }

        private void FeedStartResultFrom(int start, StringBuilder url)
        {
            url.Append("start=");
            url.Append(start);
            url.Append("&");
        }

        private void FeedMaxResults(int? size, StringBuilder url)
        {
            if (size != null)
            {
                url.Append("size=");
                url.Append(size);
                url.Append("&");
            }
        }

        private void FeedFacet(List<Facet> facets, StringBuilder url)
        {
            FeedFacetList(facets, url);

            FeedFacetConstraints(facets, url);
        }

        private void FeedFacetList(List<Facet> facets, StringBuilder url)
        {
            if (facets == null || facets.Count==0)
                return;

            url.Append("facet=");

            Facet lastItem = facets.Last();
            foreach (var facet in facets)
            {
                url.Append(facet.Name);

                if (!object.ReferenceEquals(lastItem, facet))
                    url.Append(",");
            }

            url.Append("&");
        }

        private void FeedFacetConstraints(List<Facet> facets, StringBuilder url)
        {
            Facet lastItem = facets.Last();
            foreach (var facet in facets)
            {
                FeedFacet(facet, url);
            }
        }

        private void FeedFacet(Facet facet, StringBuilder url)
        {
            if (string.IsNullOrEmpty(facet.Name))
                return;

            if(facet.TopResult != null)
            {
                url.Append("facet-");
                url.Append(facet.Name);
                url.Append("-top-n=");
                url.Append(facet.TopResult);
                url.Append("&");
            }

            if (facet.FacetContraint != null)
            {
                var param = facet.FacetContraint.GetRequestParam();
                if (param != null){
                    url.Append("facet-");
                    url.Append(facet.Name);
                    url.Append("-constraints=");
                    url.Append(param);
                    url.Append("&");
                }
            }
        }

        private void FeedFields(List<string> fields, StringBuilder url)
        {
            url.Append("return-fields=");

            foreach (var field in fields)
            {
                url.Append(field);
                url.Append("%2C");
            }

            url.Append("&");
        }

        private void FeedKeyword(string keyword, StringBuilder url)
        {
            if(!string.IsNullOrEmpty(keyword))
            {
                url.Append("q=");
                url.Append(keyword);
                url.Append("&");
            }

        }
    }
}