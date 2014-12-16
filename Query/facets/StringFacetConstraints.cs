using System.Collections.Generic;
using System.Linq;
using System.Text;
using AmazingCloudSearch.Query;

namespace AmazingCloudSearch.Query.Facets
{
    public class StringFacetConstraints : IFacetContraint
    {
        public List<string> Constraint { get; set; }

        public StringFacetConstraints()
        {
            Constraint = new List<string>();
        }

        public string AddContraint(string contrain)
        {
            Constraint.Add(contrain);
            return contrain;
        }

        public string GetRequestParam()
        {
            if (Constraint.Count == 0)
                return null;


            StringBuilder s = new StringBuilder();

            var lastItem = Constraint.Last();
            foreach (var c in Constraint)
            {
                s.Append("'");
                s.Append(c);
                s.Append("'");

                if (!object.ReferenceEquals(lastItem, c))
                    s.Append(",");
            }

            return s.ToString();
        }

    }
}