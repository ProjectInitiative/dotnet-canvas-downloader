using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace canvas_downloader
{
    public static class Config
    {
        public static string PromptBaseURL()
        {
            while (true)
            {
                Console.Write("Please enter the base URL of the Canvas LMS server:: ");
                string baseURL = Console.ReadLine();
                Uri uriResult;
                bool result = Uri.TryCreate(baseURL, UriKind.Absolute, out uriResult) 
                    && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                
                if (!result)
                {
                    Console.WriteLine("URL not valid, please try again");
                    continue;
                }
                return baseURL;
            }
        }

        public static string PromptServerName()
        {
            Console.Write("Please enter a name for this Canvas LMS server:: ");
            return Console.ReadLine();
        } 

        public static string PromptAccessToken()
        {
            Console.Write("Please enter you Canvas LMS access token:: ");
            return Console.ReadLine();
        }

        public static Dictionary<string, object> GetConfig(string appRootPath, bool cacheless=false)
        {
            return new Dictionary<string, object>();

            try
            {
                Directory.CreateDirectory(Path.GetFullPath(Path.GetDirectoryName(appRootPath)));
            }
            catch (Exception) {}


            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("AccessToken", "0000");
            values.Add("ServerName", "DefaultServer");
            values.Add("ServerURL", "https://canvas.instructure.com/");

            // Product product = new Product();
            // product.ExpiryDate = new DateTime(2008, 12, 28);

            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;

            // using (StreamWriter sw = new StreamWriter(@"C:\\Users\\KyleP\\Desktop\\appsettings.json"))
            using (StreamWriter sw = new StreamWriter(OSHelper.CombinePaths(Path.GetFullPath(Path.GetDirectoryName(appRootPath)), "appsettings.json")))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, values);
            }

            using (StreamReader sr = new StreamReader(OSHelper.CombinePaths(Path.GetFullPath(Path.GetDirectoryName(appRootPath)), "appsettings.json")))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                JObject output = (Newtonsoft.Json.Linq.JObject)serializer.Deserialize(reader);
                
                var newValues = output.ToObject<Dictionary<string, object>>();
                Console.WriteLine(newValues["AccessToken"]);
                Console.WriteLine(newValues["ServerName"]);
                Console.WriteLine(newValues["ServerURL"]);
                Console.WriteLine(OSHelper.CombinePaths(Path.GetFullPath(Path.GetDirectoryName(appRootPath)), "appsettings.json"));
            }
            // var newValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(output);
        }

    }
}