namespace AmazingCloudSearch.Contract
{
    public class CloudSearchDocument : ICloudSearchDocument
    {
        public string id { get; set; }
        public string text_relevance { get; set; }
        public string domain { get; set; }
        public string tenant { get; set; }
    }

    public interface ICloudSearchDocument
    {
        string id { get; set; }
        string text_relevance { get; set; }
        string domain { get; set; }
        string tenant { get; set; }
    }
}
