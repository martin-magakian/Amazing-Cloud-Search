namespace AmazingCloudSearch.Contract
{
    public class SearchDocument : ISearchDocument
    {
        public string id { get; set; }
        public string text_relevance { get; set; }
        public string domain { get; set; }
        public string tenant { get; set; }
    }

    public interface ISearchDocument
    {
        string id { get; set; }
        string text_relevance { get; set; }
        string domain { get; set; }
        string tenant { get; set; }
    }
}
