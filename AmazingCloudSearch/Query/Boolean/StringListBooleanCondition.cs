using AmazingCloudSearch.Enum;
using System.Collections.Generic;
using System.Text;

namespace AmazingCloudSearch.Query.Boolean
{
    public class StringListBooleanCondition : IBooleanCondition
    {
        public string Field { get; set; }
        public List<string> Conditions { get; set; }
        public ConditionType ConditionType { get; set; }

        public StringListBooleanCondition(string field, List<string> conditions, ConditionType conditionType = ConditionType.OR)
        {
            Field = field;
            Conditions = conditions;
            ConditionType = conditionType;
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

        public ConditionType GetConditionType()
        {
            return ConditionType;
        }

        public bool IsList()
        {
            return true;
        }
    }
}
