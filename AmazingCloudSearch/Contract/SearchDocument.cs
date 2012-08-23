using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace AmazingCloudSearch.Contract
{
    public class SearchDocument
    {
        public SearchDocument() //constructor
        {
        }

        [ScriptIgnoreAttribute]
        public string id { get; set; }
    }
}