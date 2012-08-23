using System.Collections.Generic;

namespace AmazingCloudSearch.Contract.Result
{
    public class Error
    {
        public string message { get; set; }
    }

    public class BasicResult
    {
        public bool IsError { get; set; }

        public string status { get; set; }
        public List<Error> errors { get; set; }
        public int adds { get; set; }
        public int deletes { get; set; }
    }
}
