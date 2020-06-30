using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;


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
    }
}
