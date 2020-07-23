
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
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
            BaseURL = baseURL;
            this.baseUri = new Uri(BaseURL);
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

        public List<Dictionary<object, object>> GetCourses()
        {
            var spinner = new ConsoleSpinner();
            List<Dictionary<object, object>> response = null;
            Task.Run(() => { 
                    Console.Write("Pulling course data...");
                    response = GetPaginated(new RestRequest("courses", Method.GET));
                    spinner.IsTaskDone = true;
                });
            spinner.Wait();
            ConsoleSpinner.ClearCurrentConsoleLine();
            return response;
        }

        public List<Dictionary<object, object>> GetCourseFolders(string courseName, string courseID)
        {
            var spinner = new ConsoleSpinner();
            List<Dictionary<object, object>> folders = null;
            Task.Run(() => { 
                    Console.Write("Pulling " + courseName + " folder structure...");
                    string folderURL = "courses/" + courseID + "/folders/";
                    folders = GetPaginated(new RestRequest(folderURL, Method.GET));
                    ConsoleSpinner.ClearCurrentConsoleLine();
                    Console.Write("Pulling " + courseName + " file structure...");
                    foreach(var folder in folders)
                    {
                        folder.Add("files", GetPaginated(new RestRequest(folderURL + folder["id"] + "files/", Method.GET)));
                    }
                    spinner.IsTaskDone = true;
                });
            spinner.Wait();
            ConsoleSpinner.ClearCurrentConsoleLine();
            return folders;
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
                    // Only add header if it does not already exist
                    // TODO: replace if exists
                    if(!request.Parameters.Any(x => x.Name == entry.Key))
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