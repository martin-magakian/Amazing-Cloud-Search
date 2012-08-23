using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AmazingCloudSearch.Query.Boolean;

namespace AmazingCloudSearch.Query.Boolean
{
    public class IntBooleanCondition : IntegerRange, IBooleanCondition
    {
        public string Field { get; set; }
        public string Condition { get; set; }

        public IntBooleanCondition(string field)
        {
            Field = field;
        }

        public void SetInterval(int from, int to)
        {
            Condition = GetInterval(from, to);
        }

        public void SetFrom(int from)
        {
            Condition = GetInterval(from, null);
        }

        public void SetTo(int to)
        {
            Condition = GetInterval(null, to);
        }

        public string GetCondictionParam()
        {
            return Field + ":" +Condition;
        }
    }
}