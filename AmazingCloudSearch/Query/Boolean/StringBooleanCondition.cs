using AmazingCloudSearch.Enum;
namespace AmazingCloudSearch.Query.Boolean
{
    public class StringBooleanCondition : IBooleanCondition
    {
        public string Field { get; set; }
        public string Condition { get; set; }
        public bool Negate { get; set; }

        public StringBooleanCondition(string field, string condition, bool negate = false)
        {
            Field = field;
            Condition = condition;
            Negate = negate;
        }

        public string GetConditionParam()
        {
            var output = "'" + Condition + "'";

            if (!string.IsNullOrWhiteSpace(Field))
                output = Field + "%3A" +  output;

            if (Negate)
                output = "(not+" + output + ")";

            return output;
        }

        public ConditionType GetConditionType()
        {
            return ConditionType.AND;
        }

        public bool IsList()
        {
            return false;
        }
    }
}
