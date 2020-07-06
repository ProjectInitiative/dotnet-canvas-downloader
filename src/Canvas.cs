
using System;
using System.Collections.Generic;
using System.Net;

namespace canvas_downloader
{

    [Serializable]
    class MalformedURLException : Exception
    {
        public MalformedURLException() { }

        public MalformedURLException(string url)
            : base(String.Format("Malformed URL: {0}", url)) { }
    }


    public class Canvas
    {
        private string _baseURL;
        public string baseURL 
        { 
            get { return this._baseURL; }
            set
            {
                Uri uriResult;
                bool result = Uri.TryCreate(value, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                if(result) 
                {
                    this._baseURL = value.TrimEnd(new[] {'/'}) + "/api/v1/";
                }
                else 
                {
                    throw new MalformedURLException(value);
                }
            } 
        }
        public Dictionary<string, string> headers { get; private set; }
        public string accessToken { get; set; }
        public string username { get; set; }
        public string password { get; set; }

        private Canvas(string baseURL)
        {
            this.baseURL = baseURL;
        }

        public Canvas(string baseURL, string username, string password)
            :this(baseURL)
        {
            
            this.username = username;
            this.password = password;
            byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes(this.username + ":" + this.password);
            this.headers = new Dictionary<string, string>
            {
                { "Authorization", "Basic " + System.Convert.ToBase64String(data) }
            };
        }

        public Canvas(string baseURL, string accessToken) 
            :this(baseURL)
        {
            this.accessToken = accessToken;
            this.headers = new Dictionary<string, string>
            {
                { "Authorization", "Bearer " + this.accessToken }
            };
        }

        // public string GetPaginated(string url, int page = -1)
        // {
        //     if (page == -1)
        //     {
        //         var response = 
        //     }
        // }

        // public bool CheckResponse(string response)
        // {

        // }

    }
}