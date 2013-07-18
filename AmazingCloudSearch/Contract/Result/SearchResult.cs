using System.Collections.Generic;
using AmazingCloudSearch.Contract.Facet;
using Newtonsoft.Json;

namespace AmazingCloudSearch.Contract.Result
{
    public class SearchResult<T> where T : ICloudSearchDocument, new()
    {
        public bool IsError { get; set; }

        public string rank { get; set; }

        [JsonProperty("match-expr")]
        public string matchExpr { get; set; }

        public Hits<T> hits { get; set; }
        public Info info { get; set; }
        public string error { get; set; }
        public List<Message> messages { get; set; }

        public List<FacetResult> facetsResults { get; set; }

        public class Message
        {
            public string severity { get; set; }
            public string code { get; set; }
            public string message { get; set; }
        }

        public class Hits<T>
        {
            public int found { get; set; }
            public int start { get; set; }
            public List<Hit<T>> hit { get; set; }
        }

        public class Hit<T>
        {
            public string id { get; set; }
            public T data { get; set; }
        }

        public class Info
        {
            public string rid { get; set; }

            [JsonProperty("time-ms")]
            public int timeMs { get; set; }

            [JsonProperty("cpu-time-ms")]
            public int cpuTimeMs { get; set; }
        }

        public List<Constraint> GetFacetResults(string name)
        {
            List<Constraint> contraints = null;

            if (facetsResults != null && facetsResults.Count > 0)
            {
                foreach (FacetResult facetResult in facetsResults)
                {
                    if (facetResult.Name == name)
                    {
                        contraints = facetResult.Contraint;
                        break;
                    }
                }
            }

            return contraints;
        }
    }
}
