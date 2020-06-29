using CommandLine;

namespace canvas_downloader 
{
    public class Options
    {
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        [Option('n', "nocache", Required = false, HelpText = "Run the program with no cached data.")]
        public bool NoCache { get; set; }
        
    }
}