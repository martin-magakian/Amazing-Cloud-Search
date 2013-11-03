using System.Collections.Generic;
using System.Linq;
using AmazingCloudSearch.Contract;
using AmazingCloudSearch.Enum;
using AmazingCloudSearch.Helper;

namespace AmazingCloudSearch.Builder
{
    public class ActionBuilder<T> where T : ICloudSearchDocument
    {
        public AddUpdateBasicDocumentAction<T> BuildAction(T document, ActionType actionType, int version)
        {
            string type = ActionTypeFunction.ActionTypeToString(actionType);

            return new AddUpdateBasicDocumentAction<T> {type = type, id = document.Id, lang = "en", fields = document, version = version};
        }

        public AddUpdateBasicDocumentAction<T> BuildAction(T document, ActionType actionType)
        {
            int version = Timestamp.CurrentTimeStamp();

            return BuildAction(document, actionType, version);
        }

        public BasicDocumentAction BuildDeleteAction(ICloudSearchDocument document, ActionType actionType, int version)
        {
            string type = ActionTypeFunction.ActionTypeToString(actionType);

            return new BasicDocumentAction {type = type, id = document.Id, version = version};
        }

        public BasicDocumentAction BuildDeleteAction(ICloudSearchDocument document, ActionType actionType)
        {
            int version = Timestamp.CurrentTimeStamp();

            return BuildDeleteAction(document, actionType, version);
        }

        public IEnumerable<BasicDocumentAction> BuildDeleteAction(List<ICloudSearchDocument> documents, ActionType actionType)
        {
            int version = Timestamp.CurrentTimeStamp();

            return documents.Select(x => BuildDeleteAction(x, actionType, version));
        }
    }
}
