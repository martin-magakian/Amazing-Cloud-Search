using AmazingCloudSearch.Contract;
using AmazingCloudSearch.Enum;
using AmazingCloudSearch.Helper;

namespace AmazingCloudSearch.Builder
{
    public class ActionBuilder<T> where T : ICloudSearchDocument
    {
        public AddUpdatBasicDocumentAction<T> BuildAction(T document, ActionType actionType, int version)
        {
            string type = ActionTypeFunction.ActionTypeToString(actionType);

            return new AddUpdatBasicDocumentAction<T> {type = type, id = document.id, lang = "en", fields = document, version = version};
        }

        public AddUpdatBasicDocumentAction<T> BuildAction(T document, ActionType actionType)
        {
            int version = Timestamp.CurrentTimeStamp();

            return BuildAction(document, actionType, version);
        }

        public BasicDocumentAction BuildDeleteAction(ICloudSearchDocument document, ActionType actionType, int version)
        {
            string type = ActionTypeFunction.ActionTypeToString(actionType);

            return new BasicDocumentAction {type = type, id = document.id, version = version};
        }

        public BasicDocumentAction BuildDeleteAction(ICloudSearchDocument document, ActionType actionType)
        {
            int version = Timestamp.CurrentTimeStamp();

            return BuildDeleteAction(document, actionType, version);
        }
    }
}
