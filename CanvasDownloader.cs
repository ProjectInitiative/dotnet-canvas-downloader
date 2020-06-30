using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using CommandLine;
using Microsoft.Extensions.Configuration;

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
    
        // Show how to create an instance of the Configuration class
        // that represents this application configuration file.  
        static void CreateConfigurationFile()
        {
            try
            {

                // Create a custom configuration section.
                CustomSection customSection = new CustomSection();

                // Get the current configuration file.
                System.Configuration.Configuration config =
                        ConfigurationManager.OpenExeConfiguration(
                        ConfigurationUserLevel.None);

                // Create the custom section entry  
                // in <configSections> group and the 
                // related target section in <configuration>.
                if (config.Sections["CustomSection"] == null)
                {
                    config.Sections.Add("CustomSection", customSection);
                }

                // Create and add an entry to appSettings section.
                
                string conStringname="LocalSqlServer";
                string conString = @"data source=.\SQLEXPRESS;Integrated Security=SSPI;AttachDBFilename=|DataDirectory|aspnetdb.mdf;User Instance=true";
                string providerName="System.Data.SqlClient";

                ConnectionStringSettings connStrSettings = new ConnectionStringSettings();
                connStrSettings.Name = conStringname;
                connStrSettings.ConnectionString= conString;
                connStrSettings.ProviderName = providerName;

                config.ConnectionStrings.ConnectionStrings.Add(connStrSettings);
                
                // Add an entry to appSettings section.
                int appStgCnt =
                    ConfigurationManager.AppSettings.Count;
                string newKey = "NewKey" + appStgCnt.ToString();

                string newValue = DateTime.Now.ToLongDateString() +
                " " + DateTime.Now.ToLongTimeString();

                config.AppSettings.Settings.Add(newKey, newValue);

                // Save the configuration file.
                customSection.SectionInformation.ForceSave = true;
                config.Save(ConfigurationSaveMode.Full);

                Console.WriteLine("Created configuration file: {0}",
                    config.FilePath);
            }
            catch (ConfigurationErrorsException err)
            {
                Console.WriteLine("CreateConfigurationFile: {0}", err.ToString());
            }
        }
    }
}
