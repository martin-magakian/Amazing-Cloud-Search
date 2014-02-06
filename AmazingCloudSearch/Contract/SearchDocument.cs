using Newtonsoft.Json;

namespace AmazingCloudSearch.Contract
{
    public class CloudSearchDocument : ICloudSearchDocument
    {
		[JsonProperty("id")]
        public string Id { get; set; }
    }

    public interface ICloudSearchDocument
    {
        string Id { get; set; }
    }
}
