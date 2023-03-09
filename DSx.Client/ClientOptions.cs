using CommandLine;

namespace DSx.Client
{
    [Verb("Client")]
    public class ClientOptions
    {
        [Option(longName: "Port", Default = null, HelpText = "Network port N and N+1 (UDP) for controller input. Defaults to input from local controller.")]
        public ushort? Port { get; set; }
        
        [Option(longName: "PollingInterval", Default = (ushort)10,  HelpText = "Polling interval for controller input")]
        public ushort PollingInterval { get; set; }
        
        [Option(longName: "Count", Default = (byte)4, Required = false, HelpText = "Number of emulated controllers to connect (max 4)")]
        public byte Count { get; set; }
        
        [Option(longName: "NoConsole", Default = false, HelpText = "Do not render console")]
        public bool NoConsole { get; set; }
        
        [Option(longName: "MappingPath", Default = null, HelpText = "Path to mapping file")]
        public string MappingPath { get; set; }
    }
}