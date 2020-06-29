using System;
using CommandLine;


namespace canvas_downloader
{
    class CanvasDownloader
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                    .WithParsed<Options>(o =>
                    {
                        if (o.Verbose)
                        {
                            Console.WriteLine($"Verbose output enabled. Current Arguments: -v {o.Verbose}");
                            Console.WriteLine("Quick Start Example! App is in Verbose mode!");
                        }
                        else
                       {
                           Console.WriteLine($"Current Arguments: -v {o.Verbose}");
                           Console.WriteLine("Quick Start Example!");
                       }
                    });
        }
    }
}
