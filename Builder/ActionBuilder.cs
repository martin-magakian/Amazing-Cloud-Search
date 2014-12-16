using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AmazingCloudSearch.Contract;
using AmazingCloudSearch.Enum;
using AmazingCloudSearch.Helper;

namespace AmazingCloudSearch.Builder
{
    public class ActionBuilder<T> where T : SearchDocument
    {

        public AddUpldateBasicDocumentAction<T> BuildAction(T document, ActionType actionType, int version)
        {
            var type = ActionTypeFunction.ActionTypeToString(actionType);

            return new AddUpldateBasicDocumentAction<T> { type = type, id = document.id, lang = "en", fields = document, version = version };
        }

        public AddUpldateBasicDocumentAction<T> BuildAction(T document, ActionType actionType)
        {
            int version = Timestamp.CurrentTimeStamp();

            return BuildAction(document, actionType, version);
        }


        public BasicDocumentAction BuildDeleteAction(SearchDocument document, ActionType actionType, int version)
        {
            var type = ActionTypeFunction.ActionTypeToString(actionType);

            return new BasicDocumentAction { type = type, id = document.id, version = version };
        }

        public BasicDocumentAction BuildDeleteAction(SearchDocument document, ActionType actionType)
        {
            int version = Timestamp.CurrentTimeStamp();

            return BuildDeleteAction(document, actionType, version);
        }
    }
}