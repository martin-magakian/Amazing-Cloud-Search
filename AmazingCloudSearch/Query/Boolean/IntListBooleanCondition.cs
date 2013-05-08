using System.Collections.Generic;
using System.Text;

namespace AmazingCloudSearch.Query.Boolean
{
    public class IntListBooleanCondition : IBooleanCondition
    {
        public string Field { get; set; }
        public List<int> Conditions { get; set; }
        public bool IsOrConditionParam { get; set; }

        public IntListBooleanCondition(string field, List<int> conditions, bool isOrConditionParam = true)
        {
            Field = field;
            Conditions = conditions;
            IsOrConditionParam = isOrConditionParam;
        }

        public string GetCondictionParam()
        {
            StringBuilder condictionParam = new StringBuilder();

            foreach (int condition in Conditions)
            {
                condictionParam.Append(Field + "%3A" + condition);
                condictionParam.Append("+");
            }

            if (condictionParam.Length > 0)
            {
                condictionParam.Remove(condictionParam.Length - 1, 1);
            }

            return condictionParam.ToString();
        }

        public bool IsOrCondition()
        {
            return IsOrConditionParam;
        }

        public bool IsList()
        {
            return true;
        }
    }
}
