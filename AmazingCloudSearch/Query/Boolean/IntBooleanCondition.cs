using AmazingCloudSearch.Enum;
namespace AmazingCloudSearch.Query.Boolean
{
    public class IntBooleanCondition : IntegerRange, IBooleanCondition
    {
        public string Field { get; set; }
        public string Condition { get; set; }
        public ConditionType ConditionType { get; set; }

        public IntBooleanCondition(string field)
        {
            Field = field;
        }

        public IntBooleanCondition(string field, int condition, ConditionType conditionType = ConditionType.OR)
        {
            Field = field;
            Condition = condition.ToString();
            ConditionType = conditionType;
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

        public string GetConditionParam()
        {
            return Field + "%3A" + Condition;
        }

        public ConditionType GetConditionType()
        {
            return ConditionType;
        }

        public bool IsList()
        {
            return false;
        }
    }
}
