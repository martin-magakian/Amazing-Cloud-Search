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
		public bool IsOrConditionParam { get; set; }

        public IntBooleanCondition(string field)
        {
            Field = field;
        }

		public IntBooleanCondition(string field, int condition, bool isOrConditionParam = true)
		{
			Field = field;
			Condition = condition.ToString();
			IsOrConditionParam = isOrConditionParam;
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
            return Field + "%3A" + Condition;
        }

		public bool IsOrCondition()
		{
			return IsOrConditionParam;
		}

		public bool IsList()
		{
			return false;
		}
    }
}