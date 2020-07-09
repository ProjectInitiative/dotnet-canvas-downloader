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

        // TODO: add in switch to handle adding multiple servers and cacheless (cacheless will take priority)
        public static List<Dictionary<string, object>> GetConfig(bool cacheless=false, bool addServer=false)
        {

            try
            {
                Directory.CreateDirectory(Path.GetFullPath(OSHelper.configFolder));
            }
            catch (Exception) {}

            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.Formatting = Formatting.Indented;
            serializer.NullValueHandling = NullValueHandling.Ignore;
            
            if (cacheless)
            {
                return WriteConfig(serializer, writeToDisk: false);
            }

            try
            {
                var servers = ReadConfig(serializer);
                // if (ContainsKeys(servers, new List<string> {"ServerName","ServerURL","AccessToken"}))
                if (true)
                {
                    if (addServer)
                    {
                        servers = WriteConfig(serializer, servers: servers, addServer: true);
                    }
                    return servers;
                }
                else
                {
                    Console.WriteLine("Configuration corrupted or missing values");
                    return WriteConfig(serializer);
                }
            }
            catch (System.IO.FileNotFoundException)
            {
                Console.WriteLine("Configuration not found");
                return WriteConfig(serializer);
            }
        }

        public static bool ContainsKeys(Dictionary<string, object> dictionary,
                             List<string> keys)
        {
            return keys.Any() 
                && keys.All(key => dictionary.ContainsKey(key));
        }

        private static List<Dictionary<string, object>> ReadConfig(JsonSerializer serializer)
        {
            List<Dictionary<string, object>> servers;
            using (StreamReader sr = new StreamReader(OSHelper.appSettings))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                JArray output = (Newtonsoft.Json.Linq.JArray)serializer.Deserialize(reader);
                servers = output.ToObject<List<Dictionary<string,object>>>();
                Console.WriteLine("Read config from " + OSHelper.appSettings);
            }
            //Add a check to verify all data exists in Json file
            return servers;
        }

        private static List<Dictionary<string, object>> WriteConfig(JsonSerializer serializer,
                                                                    List<Dictionary<string, object>> servers=null, bool writeToDisk=true, bool addServer=false)
        {
            if (servers == null)
            {
                servers = new List<Dictionary<string, object>>();
            }

            Dictionary<string, object> server = new Dictionary<string, object>();

            server.Add("ServerName", PromptServerName());
            server.Add("ServerURL", PromptServerURL());
            server.Add("AccessToken", PromptAccessToken());
            
            // append server to list of servers in the config
            servers.Add(server);
            if (writeToDisk)
            {
                if (addServer)
                    Console.WriteLine("New Server added!");

                using (StreamWriter sw = new StreamWriter(OSHelper.appSettings))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, servers);
                }
            }
            else
                Console.WriteLine("Configuration will not be saved");
            
            return servers;
        }

    }
}