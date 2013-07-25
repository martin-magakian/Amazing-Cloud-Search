namespace AmazingCloudSearch.Enum
{
    public enum ActionType
    {
        ADD,
        DELETE
    }

    public class ActionTypeFunction
    {
        public static string ActionTypeToString(ActionType type)
        {
            return type.ToString().ToLower();
        }
    }
}
