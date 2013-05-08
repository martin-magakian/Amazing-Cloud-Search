using System.Collections.Generic;
using AmazingCloudSearch.Contract.Facet;

namespace AmazingCloudSearch.Contract.Result
{
    public class FacetResult
    {
        public string Name { get; set; }
        public List<Constraint> Contraint { get; set; }
    }
}
