using CommandLine;

namespace canvas_downloader 
{
    public class Options
    {
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }
        
        [Option('o', "output", Required = false, HelpText = "Specify output directory.")]
        public string Output { get; set; }
        
[       Option('m', "nomodules", Required = false, HelpText = "Run program without downloading modules.")]
        public bool NoModules { get; set; }
        
        [Option('f', "nofiles", Required = false, HelpText = "Run program without downloading files.")]
        public bool NoFiles { get; set; }

        [Option('n', "nocache", Required = false, HelpText = "Run program without cached credentials.")]
        public bool NoCache { get; set; }
        
        [Option('x', "DeleteCache", Required = false, HelpText = "Deletes all cached login information.")]
        public bool DeleteCache { get; set; }
        
    }
}