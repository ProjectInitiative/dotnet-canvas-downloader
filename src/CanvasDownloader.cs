using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
// using Microsoft.Extensions.Configuration;

namespace canvas_downloader
{
    class CanvasDownloader
    {
        static Options opts { get; set; }
        static Dictionary<string,object> configs = null;
        private static string appRootPath { get; } = Path.GetFullPath(
            Path.GetDirectoryName(System.Reflection.Assembly
                .GetExecutingAssembly().Location));
        static string rootPath { get; set; } = OSHelper.CombinePaths(
            Path.GetFullPath(Path.GetDirectoryName(appRootPath)), "courses");
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

                if (opts.NoCache) 
                    { configs = Config.GetConfig(appRootPath, true); }

                //check if the provided filepath if valid
                if (opts.Output != null)
                {
                    try
                    {
                        rootPath = OSHelper.CombinePaths(Path.GetFullPath(opts.Output), "courses");
                        Directory.CreateDirectory(rootPath);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Invalid file path. Please enter a valid path.");
                        return;
                    }
                }
            }
            if (configs == null)
                { configs = Config.GetConfig(appRootPath); }
        }

        static void HandleParseError(IEnumerable<Error> errs)
        { }
    }
}
