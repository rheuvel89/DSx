using CommandLine;

namespace DSx.Host
{
    [Verb("Host")]
    public class HostOptions
    {
        [Option(longName: "Port", Default = null, HelpText = "Network port N and N+1 (UDP) for collector input. Defaults to input from local controller.")]
        public ushort? Port { get; set; }
        
        [Option(longName: "PollingInterval", Default = (ushort)10,  HelpText = "Polling interval for collector input")]
        public ushort PollingInterval { get; set; }
        
        [Option(longName: "Count", Default = (byte)4, Required = false, HelpText = "Number of emulated controllers to connect (max 4)")]
        public byte Count { get; set; }
        
        [Option(longName: "NoConsole", Default = false, HelpText = "Do not render console")]
        public bool NoConsole { get; set; }
        
        [Option(longName: "MappingPath", Default = null, HelpText = "Path to mapping file")]
        public string? MappingPath { get; set; }
        
        [Option(longName: "PluginPath", Default = null, HelpText = "Path to plugin dll's")]
        public string? PluginPath { get; set; }
    }
}