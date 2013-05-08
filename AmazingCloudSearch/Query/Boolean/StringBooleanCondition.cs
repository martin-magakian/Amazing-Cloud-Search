namespace AmazingCloudSearch.Query.Boolean
{
    public class StringBooleanCondition : IBooleanCondition
    {
        public string Field { get; set; }
        public string Condition { get; set; }

        public StringBooleanCondition(string field, string condition)
        {
            Field = field;
            Condition = condition;
        }

        public string GetCondictionParam()
        {
            return Field + "%3A" + "'" + Condition + "'";
        }

        public bool IsOrCondition()
        {
            return false;
        }

        public bool IsList()
        {
            return false;
        }
    }
}
