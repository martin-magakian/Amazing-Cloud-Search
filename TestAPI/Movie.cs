using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using AmazingCloudSearch.Contract;
using Newtonsoft.Json;

namespace TestProject1
{

    [DataContract]
    public class Movie : SearchDocument
    {
        public List<string> actor { get; set; }
        public string director { get; set; }
        public DateTime mydate { get; set; }
        public string title { get; set; }
        public int year { get; set; }
    }
}