using System.Collections.Generic;
using System.Text;

namespace AmazingCloudSearch.Query.Facets
{
    public class IntFacetContraints : IntegerRange, IFacetContraint
    {
        public List<string> Constraint { get; set; }

        public IntFacetContraints()
        {
            Constraint = new List<string>();
        }

        public void AddInterval(int from, int to)
        {
            Constraint.Add(GetInterval(from, to));
        }

        public void AddFrom(int from)
        {
            Constraint.Add(GetInterval(from, null));
        }

        public void AddTo(int to)
        {
            Constraint.Add(GetInterval(null, to));
        }

        public string GetRequestParam()
        {
            StringBuilder s = new StringBuilder();
            for (int i = 0; i != Constraint.Count; i++)
            {
                s.Append(Constraint[i]);
                if (Constraint.Count != i)
                    s.Append(",");
            }
            return s.ToString();
        }
    }
}