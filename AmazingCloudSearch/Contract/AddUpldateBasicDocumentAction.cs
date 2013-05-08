namespace AmazingCloudSearch.Contract
{
    public class AddUpldateBasicDocumentAction<T> : BasicDocumentAction where T : ISearchDocument
    {
        public string lang { get; set; }
        public T fields { get; set; }
    }
}
