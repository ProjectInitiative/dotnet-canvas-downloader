using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;

namespace canvas_downloader
{
    class CanvasDownloader
    {
        private static Options opts;
        private static List<Dictionary<string,object>> configs = null;
        
        private static string coursesPath = OSHelper.CombinePaths(
            Path.GetFullPath(Path.GetDirectoryName(OSHelper.AppRootPath)), "courses");
        static void Main(string[] args)
        {
            //parse command line arguments
            CommandLine.Parser.Default.ParseArguments<Options>(args)
                .WithParsed((Options o ) => { opts = o; })
                .WithNotParsed(HandleParseError);
            if (opts != null)
            {
                if (opts.DeleteCache) 
                    { 
                        try 
                        {
                            File.Delete(OSHelper.AppSettings);
                            Console.WriteLine("Successfully deleted {0}", OSHelper.AppSettings);
                        } 
                        catch (Exception)
                        {
                            Console.WriteLine("Error deleting {0}", OSHelper.AppSettings);
                            Environment.Exit(1);
                        }
                    }

                //check if the provided filepath if valid
                if (opts.Output != null)
                {
                    try
                    {
                        coursesPath = OSHelper.CombinePaths(Path.GetFullPath(opts.Output), "courses");
                        Directory.CreateDirectory(coursesPath);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Invalid file path. Please enter a valid path.");
                        Environment.Exit(1);
                    }
                }


                if (opts.NoCache) 
                    { configs = Config.GetConfig(cacheless: true); }
                
                else if (opts.AddServer) 
                    { configs = Config.GetConfig(addServer: true); }
                
                if (opts.NoFiles && opts.NoModules)
                    { Environment.Exit(0); }
                
            }
            if (configs == null)
                { configs = Config.GetConfig(); }


            string baseURL = (string)configs[0]["ServerURL"];
            string accessToken = (string)configs[0]["AccessToken"];


            var canvas = new Canvas(baseURL, accessToken);
            var courses = canvas.GetCourses();
            foreach (var course in courses)
            {
                if (course.ContainsKey("name") && course.ContainsKey("id"))
                {
                    Console.WriteLine("Course: " + course["name"]);
                    course.Add("folders", canvas.GetCourseFolders(course["id"].ToString()));
                    if (((List<Dictionary<object, object>>)course["folders"]).Count != 0  && !opts.NoFiles)
                    {
                        foreach (var folder in (List<Dictionary<object, object>>)course["folders"])
                        {
                            if (folder.ContainsKey("name"))
                            {
                                Console.WriteLine("\tFolder: " + folder["name"]);
                            }
                        }
                    }
                }
            }
            Console.WriteLine(courses.Count);
        }

        static void HandleParseError(IEnumerable<Error> errs)
        { 
            Environment.Exit(1);
        }
    }
}
