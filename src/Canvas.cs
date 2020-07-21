
using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using RestSharp;

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
        private string baseURL;
        public string BaseURL 
        { 
            get { return baseURL; }
            set
            {
                Uri uriResult;
                bool result = Uri.TryCreate(value, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                if(result) 
                {
                    baseURL = value.TrimEnd(new[] {'/'}) + "/api/v1/";
                }
                else 
                {
                    throw new MalformedURLException(value);
                }
            } 
        }
        private Uri baseUri;
        private IRestClient client;
        private Dictionary<string, string> headers;
        private string accessToken;
        private string username;
        private string password;

            

        private Canvas(string baseURL)
        {
            this.baseURL = baseURL;
            this.baseUri = new Uri(this.baseURL);
            this.client = new RestClient(this.baseUri);
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


        public List<Dictionary<object, object>> GetPaginated(IRestRequest request, int page = -1)
        {
            IRestResponse response;
            AddHeaders(request);

            if (page == -1)
            {
                response = client.Get(request);
                page = 1;
            }
            else
            {
                request.AddQueryParameter("page", page.ToString());
                response = client.Get(request);
            }

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var data = JsonConvert.DeserializeObject<List<Dictionary<object, object>>>(response.Content);
                if (data.Count == 0)
                { 
                    return new List<Dictionary<object, object>>(); 
                }
                data.AddRange((GetPaginated(request, page=page+1)));
                return data;
            }
            else
            {
                return new List<Dictionary<object, object>>();
            }
        }

        private void AddHeaders(IRestRequest request)
        {
            if (headers != null)
            {
                foreach(KeyValuePair<string, string> entry in headers)
                {
                    request.AddHeader(entry.Key, entry.Value);
                }
            }
        }

        public string AccessToken { get => accessToken; set { accessToken = value; } }
        public string Username { get => username; set { username = value; } }
        public string Password { get => password; set { password = value; } }

        public Dictionary<string, string> Headers { get => headers; }

    }
}