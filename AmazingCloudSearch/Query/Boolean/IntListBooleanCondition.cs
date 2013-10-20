using AmazingCloudSearch.Enum;
using System.Collections.Generic;
using System.Text;

namespace AmazingCloudSearch.Query.Boolean
{
    public class IntListBooleanCondition : IBooleanCondition
    {
        public string Field { get; set; }
        public List<int> Conditions { get; set; }
        public ConditionType ConditionType { get; set; }

        public IntListBooleanCondition(string field, List<int> conditions, ConditionType conditionType = ConditionType.OR)
        {
            Field = field;
            Conditions = conditions;
            ConditionType = conditionType;
        }

        public string GetConditionParam()
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
