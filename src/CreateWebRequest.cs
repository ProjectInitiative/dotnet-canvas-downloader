using System;
using System.IO;
using System.Net;
using System.Text;

namespace canvas_downloader
{
    public class CreateWebRequest
    {
        private WebRequest request;
            private Stream dataStream;
 
            private string status;
 
            public String Status
            {
                get
                {
                    return status;
                }
                set
                {
                    status = value;
                }
            }
 
            public CreateWebRequest(string url)
            {
                // Create a request using a URL that can receive a post.
 
                this.request = WebRequest.Create(url);
            }
 
            public CreateWebRequest(string url, string method)
                : this(url)
            {
 
                if (method.Equals("GET") || method.Equals("POST"))
                {
                    // Set the Method property of the request to POST.
                    this.request.Method = method;
                }
                else
                {
                    throw new Exception("Invalid Method Type");
                }
            }
 
            public CreateWebRequest(string url, string method, string data)
                : this(url, method)
            {
 
                // Create POST data and convert it to a byte array.
                string postData = data;
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
 
                // Set the ContentType property of the WebRequest.
                this.request.ContentType = "application/x-www-form-urlencoded";
 
                // Set the ContentLength property of the WebRequest.
                this.request.ContentLength = byteArray.Length;
 
                // Get the request stream.
                this.dataStream = request.GetRequestStream();
 
                // Write the data to the request stream.
                this.dataStream.Write(byteArray, 0, byteArray.Length);
 
                // Close the Stream object.
                this.dataStream.Close();
 
            }
 
            public string GetResponse()
            {
                // Get the original response.
                WebResponse response = request.GetResponse();
 
                this.Status = ((HttpWebResponse)response).StatusDescription;
 
                // Get the stream containing all content returned by the requested server.
                dataStream = response.GetResponseStream();
 
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
 
                // Read the content fully up to the end.
                string responseFromServer = reader.ReadToEnd();
 
                // Clean up the streams.
                reader.Close();
                dataStream.Close();
                response.Close();
 
                return responseFromServer;
            }
    }
}