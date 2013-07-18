namespace AmazingCloudSearch.Contract
{
    public class AddUpdateBasicDocumentAction<T> : BasicDocumentAction where T : ICloudSearchDocument
    {
        public string lang { get; set; }
        public T fields { get; set; }
    }
}
