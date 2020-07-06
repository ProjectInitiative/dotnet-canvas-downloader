using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
// using Microsoft.Extensions.Configuration;

namespace canvas_downloader
{
    class CanvasDownloader
    {
        static Options opts { get; set; }
        private static string appRootPath { get; } = System.Reflection.Assembly.GetExecutingAssembly().Location;
        static string rootPath { get; set; } = CombinePaths(Path.GetFullPath(Path.GetDirectoryName(appRootPath)), "courses");
        static void Main(string[] args)
        {
            //parse command line arguments
            CommandLine.Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunOptions)
                .WithNotParsed(HandleParseError);
            if (opts != null)
            {
                
                if (opts.NoFiles && opts.NoModules)
                {
                    return;
                }

                //check if the provided filepath if valid
                if (opts.Output != null)
                {
                    try
                    {
                        rootPath = CombinePaths(Path.GetFullPath(opts.Output), "courses");
                        Directory.CreateDirectory(rootPath);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Invalid file path. Please enter a valid path.");
                        return;
                    }
                }
            }
            // CreateConfigurationFile();
            // GetConfigurationFile();
            Test();
        }

        static string CombinePaths(params string[] paths)
        {
            if (paths == null)
            {
                throw new ArgumentNullException("paths");
            }
            return paths.Aggregate(Path.Combine);
        }
        static void RunOptions(Options o)
        {
            // Parser.Default.ParseArguments<Options>(args)
            //         .WithParsed<Options>(o =>
            //         {
            //             opts = o;
            //         });
            opts = o;
        }
        static void HandleParseError(IEnumerable<Error> errs)
        {
            
        }
        static void FormatFileName(string fileName)
        {
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }
        }

        static void Test()
        {

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
            using (StreamWriter sw = new StreamWriter(CombinePaths(Path.GetFullPath(Path.GetDirectoryName(appRootPath)), "appsettings.json")))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, values);
            }

            // Dictionary<string, object> newValues;

            // string output;

            using (StreamReader sr = new StreamReader(CombinePaths(Path.GetFullPath(Path.GetDirectoryName(appRootPath)), "appsettings.json")))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                JObject output = (Newtonsoft.Json.Linq.JObject)serializer.Deserialize(reader);
                
                var newValues = output.ToObject<Dictionary<string, object>>();
                Console.WriteLine(newValues["AccessToken"]);
                Console.WriteLine(newValues["ServerName"]);
                Console.WriteLine(newValues["ServerURL"]);
                Console.WriteLine(CombinePaths(Path.GetFullPath(Path.GetDirectoryName(appRootPath)), "appsettings.json"));
            }
            // var newValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(output);
        }


    }
}
