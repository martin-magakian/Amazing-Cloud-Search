namespace AmazingCloudSearch.Contract
{
    public class CloudSearchDocument : ICloudSearchDocument
    {
        public string id { get; set; }
    }

    public interface ICloudSearchDocument
    {
        string id { get; set; }
    }
}
