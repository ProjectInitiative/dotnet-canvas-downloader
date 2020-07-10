using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
// using Microsoft.Extensions.Configuration;

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
                
                if (opts.NoFiles && opts.NoModules)
                { return; }

                if (opts.DeleteCache) 
                    { 
                        try 
                        {
                            File.Delete(OSHelper.AppSettings);
                            Console.WriteLine("Successfully deleted {0}", OSHelper.AppSettings);
                        } 
                        catch (Exception e)
                        {
                            Console.WriteLine("Error deleting {0}", OSHelper.AppSettings);
                        }
                        return;
                    }

                if (opts.NoCache) 
                    { configs = Config.GetConfig(cacheless: true); }
                else if (opts.AddServer) 
                    { configs = Config.GetConfig(addServer: true); }

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
                        return;
                    }
                }
            }
            if (configs == null)
                { configs = Config.GetConfig(); }
        }

        static void HandleParseError(IEnumerable<Error> errs)
        { }
    }
}
