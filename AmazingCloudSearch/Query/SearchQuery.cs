using System.Collections.Generic;
using System.Reflection;
using System.Text;
using AmazingCloudSearch.Contract;
using AmazingCloudSearch.Helper;
using AmazingCloudSearch.Query.Boolean;
using AmazingCloudSearch.Query.Facets;

namespace AmazingCloudSearch.Query
{
    public class SearchQuery<T> where T : SearchDocument, new()
    {
        public BooleanQuery BooleanQuery { get; set; }

        public string Keyword { get; set; }

        public List<Facet> Facets { get; set; }

        public List<string> Fields { get; set; }

        public int Start { get; set; }

        public int? Size { get; set; }

        public SearchQuery()
        {
            BuildPropertiesArray(new ListProperties<T>().GetProperties());
        }

        private void BuildPropertiesArray(List<PropertyInfo> properties)
        {
            var li = new List<string>();

            foreach (var property in properties)
            {
                li.Add(property.Name);
            }

            Fields = li;
        }
    }
}