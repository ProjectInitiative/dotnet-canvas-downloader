using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace canvas_downloader
{
    public static class OSHelper
    {

        // directory that the exe is extracted to during execution
        private static string appRootPath = Path.GetFullPath(
            Path.GetDirectoryName(System.Reflection.Assembly
                .GetExecutingAssembly().Location));

        private static string configFolder = OSHelper.CombinePaths(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"canvas-downloader");
        
        private static string appSettings = OSHelper.CombinePaths(Path.GetFullPath(configFolder), "appsettings.json");

        // OS way of combining multiple strings together to create a valid path
        public static string CombinePaths(params string[] paths)
        {
            if (paths == null)
            {
                throw new ArgumentNullException("paths");
            }
            return paths.Aggregate(Path.Combine);
        }

        public static void MakeFolder(string path)
        {
            try
            {
                // Determine whether the directory exists.
                if (Directory.Exists(path))
                { return; }
                // Try to create the directory.
                DirectoryInfo di = Directory.CreateDirectory(path);
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }

        public static string SanitizeFileName(string fileName, char replacementChar = '_')
        {
            var blackList = new HashSet<char>(System.IO.Path.GetInvalidFileNameChars());
            var output = fileName.ToCharArray();
            for (int i = 0, ln = output.Length; i < ln; i++)
            {
                if (blackList.Contains(output[i]))
                {
                    output[i] = replacementChar;
                }
            }
            return new String(output);
        }

        public static string ConfigFolder { get => configFolder; }
        public static string AppSettings { get => appSettings; }
        public static string AppRootPath { get => appRootPath; }

    }
}