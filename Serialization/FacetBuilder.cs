using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using AmazingCloudSearch.Contract.Facet;
using AmazingCloudSearch.Contract.Result;
using Newtonsoft.Json;

namespace AmazingCloudSearch.Serialization
{
    class FacetBuilder
    {
        private JavaScriptSerializer _serializer;

        public FacetBuilder()
        {
            _serializer = new JavaScriptSerializer();
        }

        public List<FacetResult> BuildFacet(dynamic jsonDynamic)
        {
            try{
                return BuildFacetWithException(jsonDynamic);
            }catch(Exception e)
            {
                return new List<FacetResult>();
            }
        }

        private List<FacetResult> BuildFacetWithException(dynamic jsonDynamic)
        {
            dynamic facets = jsonDynamic.facets;
            if (facets == null)
                return null;

            string jsonFacet = facets.ToString();

            var facetDictionary = _serializer.Deserialize<Dictionary<string, object>>(jsonFacet);

            var liFacet = new List<FacetResult>();

            foreach (KeyValuePair<string, object> pair in facetDictionary)
            {
                var contraints = CreateContraint(pair);
                liFacet.Add(new FacetResult { Name = pair.Key, Contraint = contraints.constraints });
            }

            return liFacet;
        }

        private Constraints CreateContraint(KeyValuePair<string, object> pair)
        {
            try
            {
                var tmpContraints = JsonConvert.SerializeObject(pair.Value);
                var contraints = JsonConvert.DeserializeObject<Constraints>(tmpContraints.ToString());
                return contraints;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private Constraint BuildContraint(string key, object value)
        {
            var valueDic = (Dictionary<string, object>)value;

            List<object> first = null;

            foreach (var pair in valueDic)
            {
                first = (List<object>)pair.Value;
                Console.Write("");
            }

            return null;
        }
    }
}
