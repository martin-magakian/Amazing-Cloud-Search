using System;
using System.Collections.Generic;

namespace AmazingCloudSearch.Helper
{
    public class ConvertArray
    {

        public List<int> StringToInt(List<string> newValues)
        {
            var r = new List<int>();

            foreach (var entry in newValues)
            {
                int value;
                if (int.TryParse(entry, out value))
                    r.Add(value);
            }

            return r;
        }

        public List<int?> StringToIntNull(List<string> newValues)
        {
            var r = new List<int?>();

            foreach (var entry in newValues)
            {
                int value;
                if (int.TryParse(entry, out value))
                    r.Add(value);
                else
                    r.Add(null);
            }

            return r;
        }

        public List<DateTime> StringToDate(List<string> newValues)
        {
            var r = new List<DateTime>();

            foreach (var entry in newValues)
                r.Add(Convert.ToDateTime(entry));

            return r;
        }


        public List<string> IntToString(List<int> contraints)
        {
            var r = new List<string>();

            foreach (var entry in contraints)
                r.Add(entry.ToString());

            return r;
        }
    }
}