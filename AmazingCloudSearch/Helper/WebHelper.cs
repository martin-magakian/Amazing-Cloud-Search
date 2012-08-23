using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace AmazingCloudSearch.Helper
{
    class WebHelper
    {

        private static string JSON_ERROR = "error";

        public class JsonResult
        {
            public bool IsError { get; set; }
            public string exeption { get; set; }
            public string json { get; set; }
        }

        public JsonResult PostRequest(string url, string json)
        {
            try
            {
                string rawJsonResult = PostRequestWithException(url, json);

                if (string.IsNullOrEmpty(rawJsonResult) || rawJsonResult.Equals(JSON_ERROR))
                    return new JsonResult {json = rawJsonResult, exeption = "unknow error", IsError = true};

                return new JsonResult { json = rawJsonResult };
            }
            catch(Exception ex)
            {
                return new JsonResult { exeption = ex.Message, IsError = true };
            }
        }

        public JsonResult GetRequest(string url)
        {
            try
            {
                string rawJsonResult = GetRequestWithException(url);

                if (string.IsNullOrEmpty(rawJsonResult) || rawJsonResult.Equals(JSON_ERROR))
                    return new JsonResult { json = rawJsonResult, exeption = "unknow error", IsError = true };

                return new JsonResult { json = rawJsonResult };
            }
            catch (Exception ex)
            {
                return new JsonResult { exeption = ex.Message, IsError = true };
            }
        }

        private string PostRequestWithException(string url, string json)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);

            request.ProtocolVersion = HttpVersion.Version11;
            request.Method = "POST";

            byte[] postBytes = Encoding.ASCII.GetBytes(json);

            request.ContentType = "application/json";
            request.ContentLength = postBytes.Length;

            var requestStream = request.GetRequestStream();

            requestStream.Write(postBytes, 0, postBytes.Length);
            requestStream.Close();

            return RunResponse(request);
        }



        private string GetRequestWithException(string url)
        {
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.ProtocolVersion = HttpVersion.Version11;
            request.Method = "GET";
            request.Accept = "application/json";

            return RunResponse(request);
        }

        private string RunResponse(HttpWebRequest request)
        {
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException wex)
            {
                if (wex.Response == null)
                    return JSON_ERROR;

                using (var errorResponse = (HttpWebResponse)wex.Response)
                {
                    using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                    {
                        return reader.ReadToEnd(); //expected error from JSON
                    }
                }
            }

            var retVal = new StreamReader(stream: response.GetResponseStream()).ReadToEnd();

            return retVal;
        }
    }
}