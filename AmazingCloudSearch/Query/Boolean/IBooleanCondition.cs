namespace AmazingCloudSearch.Query.Boolean
{
    public interface IBooleanCondition
    {
        string GetConditionParam();
        bool IsOrCondition();
        bool IsList();
    }
}
