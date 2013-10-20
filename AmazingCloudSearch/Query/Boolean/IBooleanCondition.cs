using AmazingCloudSearch.Enum;
namespace AmazingCloudSearch.Query.Boolean
{
    public interface IBooleanCondition
    {
        string GetConditionParam();
        ConditionType GetConditionType();
        bool IsList();
    }
}
