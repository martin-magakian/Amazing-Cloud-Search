using System.Collections.Generic;
using System.Text;

namespace AmazingCloudSearch.Query.Boolean
{
    public class StringListBooleanCondition : IBooleanCondition
    {
        public string Field { get; set; }
        public List<string> Conditions { get; set; }
        public bool IsOrConditionParam { get; set; }

        public StringListBooleanCondition(string field, List<string> conditions, bool isOrConditionParam = true)
        {
            Field = field;
            Conditions = conditions;
            IsOrConditionParam = isOrConditionParam;
        }

        public string GetConditionParam()
        {
            StringBuilder condictionParam = new StringBuilder();

            foreach (string condition in Conditions)
            {
                condictionParam.Append(Field + "%3A" + "'" + condition + "'");
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
