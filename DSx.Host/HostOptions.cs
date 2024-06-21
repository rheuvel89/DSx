using CommandLine;

namespace DSx.Host
{
    [Verb("Host")]
    public class HostOptions
    {
        [Option(longName: "CollectorPort", Default = null, HelpText = "Network port N and N+1 (UDP) for collector input. Defaults to input from local controller.")]
        public ushort? CollectorPort { get; set; }
        
        [Option(longName: "PollingInterval", Default = (ushort)10,  HelpText = "Polling interval for local collector input")]
        public ushort PollingInterval { get; set; }

        [Option(longName: "Receiver", HelpText = "Host address for the output receiver.")]
        public string? Receiver { get; set; } = null;
        
        [Option(longName: "ReceiverPort", HelpText = "Network port N and N+1 (UDP) for outgoing connecitons.")]
        public ushort? ReceiverPort { get; set; }
        
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