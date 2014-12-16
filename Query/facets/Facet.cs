using AmazingCloudSearch.Query;

namespace AmazingCloudSearch.Query.Facets
{
    public class Facet
    {
        public string Name { get; set; }
        public IFacetContraint FacetContraint { get; set; }
        public int? TopResult { get; set; }
    }
}