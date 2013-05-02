using System.Web.Script.Serialization;

namespace AmazingCloudSearch.Contract
{
    public class SearchDocument : ISearchDocument
    {
        [ScriptIgnoreAttribute]
        public string id { get; set; }
    }

    public interface ISearchDocument
    {        
        string id { get; set; }
    }
}