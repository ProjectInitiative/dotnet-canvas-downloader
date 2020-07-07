using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace canvas_downloader
{
    public static class Config
    {
        public static string PromptServerURL()
        {
            while (true)
            {
                Console.Write("Please enter the base URL of the Canvas LMS server:: ");
                string serverURL = Console.ReadLine();
                Uri uriResult;
                bool result = Uri.TryCreate(serverURL, UriKind.Absolute, out uriResult) 
                    && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                
                if (!result)
                {
                    Console.WriteLine("URL not valid, please try again");
                    continue;
                }
                return serverURL;
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

        public static Dictionary<string, object> GetConfig(bool cacheless=false)
        {
            
            
            string configFolder = OSHelper.CombinePaths(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"canvas-downloader");

            try
            {
                Directory.CreateDirectory(Path.GetFullPath(configFolder));
            }
            catch (Exception) {}

            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            
            if (cacheless)
            {
                return WriteConfig(serializer, configFolder, false);
            }

            try
            {
                var values = ReadConfig(serializer, configFolder);
                if (ContainsKeys(values, new List<string> {"ServerName","ServerURL","AccessToken"}))
                {
                    return values;
                }
                else
                {
                    Console.WriteLine("Configuration corrupted or missing values");
                    return WriteConfig(serializer, configFolder);
                }
            }
            catch (System.IO.FileNotFoundException)
            {
                return WriteConfig(serializer, configFolder);
            }
        }

        public static bool ContainsKeys(Dictionary<string, object> dictionary,
                             List<string> keys)
        {
            return keys.Any() 
                && keys.All(key => dictionary.ContainsKey(key));
        }

        private static Dictionary<string, object> ReadConfig(JsonSerializer serializer, string configFolder)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            using (StreamReader sr = new StreamReader(OSHelper.CombinePaths(Path.GetFullPath(configFolder), "appsettings.json")))
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    JObject output = (Newtonsoft.Json.Linq.JObject)serializer.Deserialize(reader);
                    
                    values = output.ToObject<Dictionary<string, object>>();
                    Console.WriteLine("Read config from " + OSHelper.CombinePaths(Path.GetFullPath(configFolder), "appsettings.json"));
                }
                //Add a check to verify all data exists in Json file
                return values;
        }

        private static Dictionary<string, object> WriteConfig(JsonSerializer serializer, string configFolder, bool writeToDisk=true)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            if (writeToDisk)
                Console.WriteLine("Configuration not found");
            else
                Console.WriteLine("Configuration will not be saved");
                values.Add("ServerName", PromptServerName());
                values.Add("ServerURL", PromptServerURL());
                values.Add("AccessToken", PromptAccessToken());

            if (writeToDisk)
            {
                using (StreamWriter sw = new StreamWriter(OSHelper.CombinePaths(Path.GetFullPath(configFolder), "appsettings.json")))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, values);
                }
            }
            return values;
        }

    }
}