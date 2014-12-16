using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AmazingCloudSearch.Contract.Facet;

namespace AmazingCloudSearch.Contract.Result
{
    public class FacetResult
    {
        public string Name { get; set; }
        public List<Constraint> Contraint { get; set; }
    }
}