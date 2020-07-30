﻿using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;

namespace canvas_downloader
{
    class CanvasDownloader
    {
        private static Options opts;
        private static List<Dictionary<string,object>> configs = null;

        private static Canvas canvas;
        
        private static string coursesPath = OSHelper.CombinePaths(
            Path.GetFullPath(Directory.GetCurrentDirectory()), "courses");
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
                        OSHelper.MakeFolder(coursesPath);
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


            canvas = new Canvas(baseURL, accessToken);
            var courses = canvas.GetCourses();
            foreach (var course in courses)
            {
                if (course.ContainsKey("name") && course.ContainsKey("id"))
                {
                    Console.WriteLine("Course: " + CourseName(course));
                    
                    
                    if (!opts.NoFiles)
                    {
                        // Get folder structure in each course
                        course.Add("folders", canvas.GetCourseFolders(CourseName(course), course["id"].ToString()));
                        // Create file folder structure
                        if (((List<Dictionary<object, object>>)course["folders"]).Count != 0)
                        {
                            foreach (var folder in (List<Dictionary<object, object>>)course["folders"])
                            {
                                if (folder.ContainsKey("name"))
                                {
                                    Console.WriteLine("\tFolder: " + folder["name"]);
                                    string folderPath = OSHelper.CombinePaths(coursesPath, 
                                        OSHelper.SanitizeFileName(CourseName(course)), 
                                        "files", OSHelper.SanitizeFileName(folder["name"].ToString()));
                                    
                                    foreach (var item in (List<Dictionary<object, object>>)folder["files"])
                                    {
                                        var fileName = OSHelper.SanitizeFileName((string)item["display_name"]);
                                        if (!opts.Force && File.Exists(OSHelper.CombinePaths(folderPath, fileName)))
                                        {
                                            if (opts.Verbose)
                                            {
                                                Console.WriteLine("\t\tFile: skipping {0} already exists", fileName);
                                            }
                                        }
                                        else
                                        {
                                            if (opts.Verbose)
                                            {
                                                Console.WriteLine("\t\tFile: {0}", fileName);
                                            }
                                            canvas.DownloadFile((string)item["url"], folderPath, fileName);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (!opts.NoModules)
                    {
                        // Get module structure in each course
                        course.Add("modules", canvas.GetCourseModules(CourseName(course), course["id"].ToString()));
                        // Create module structure
                        if (((List<Dictionary<object, object>>)course["modules"]).Count != 0)
                        {
                            foreach (var module in (List<Dictionary<object, object>>)course["modules"])
                            {
                                if (module.ContainsKey("name"))
                                {
                                    Console.WriteLine("\tModule: " + module["name"]);
                                        string modulePath = OSHelper.CombinePaths(coursesPath, 
                                            OSHelper.SanitizeFileName(CourseName(course)), 
                                            "modules", OSHelper.SanitizeFileName(module["name"].ToString()));

                                    foreach (var item in (List<Dictionary<object, object>>)module["items"]) 
                                    {
                                        foreach (var modulePage in (List<Dictionary<object, object>>)item["modulePage"])
                                        {
                                            var pagePath = OSHelper.CombinePaths(modulePath, item["type"].ToString());
                                            var htmlPage = OSHelper.SanitizeFileName(item["title"].ToString() + ".html");
                                            
                                            if (!opts.Force && File.Exists(OSHelper.CombinePaths(modulePath, htmlPage)))
                                            {
                                                if (opts.Verbose)
                                                {
                                                    Console.WriteLine("\t\tModule Page: skipping {0} already exists", htmlPage);
                                                }
                                            }
                                            else
                                            {
                                                if (opts.Verbose)
                                                {
                                                    Console.WriteLine("\t\tModule page: {0}", htmlPage);
                                                }
                                                if (modulePage.ContainsKey("body"))
                                                {
                                                    canvas.WriteBodyToHTML(modulePage["body"].ToString(), pagePath, htmlPage);
                                                }
                                                else if (modulePage.ContainsKey("description"))
                                                {
                                                    canvas.WriteBodyToHTML(modulePage["description"].ToString(), pagePath, htmlPage);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                } // if course contains "name"
            } // foreach course
        } // Main

        static void HandleParseError(IEnumerable<Error> errs)
        { 
            Environment.Exit(1);
        }

        static string CourseName(Dictionary<object, object> course)
        {
            object value;
            if (course.TryGetValue("original_name", out value))
            {
                return (string)value;
            }
            else if (course.TryGetValue("name", out value))
            {
                return (string)value;
            }
            return "";
        }


    }
}
