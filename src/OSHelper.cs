using System;
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

        // replaces all invalid characters in a string to make the filename OS file system safe
        public  static void FormatFileName(string fileName)
        {
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }
        }

        public static string ConfigFolder { get => configFolder; }
        public static string AppSettings { get => appSettings; }
        public static string AppRootPath { get => appRootPath; }

    }
}