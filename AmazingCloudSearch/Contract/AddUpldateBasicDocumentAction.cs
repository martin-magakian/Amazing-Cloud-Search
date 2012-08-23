using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace AmazingCloudSearch.Contract
{

    public class AddUpldateBasicDocumentAction<T> : BasicDocumentAction where T : SearchDocument
    {
        public string lang { get; set; }
        public T fields { get; set; }
    }
}
