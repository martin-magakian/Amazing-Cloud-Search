namespace AmazingCloudSearch.Query.Boolean
{
    public interface IBooleanCondition
    {
        string GetCondictionParam();
        bool IsOrCondition();
        bool IsList();
    }
}
