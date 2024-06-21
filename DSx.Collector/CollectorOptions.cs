using CommandLine;

namespace DSx.Collector
{
    [Verb("Collector")]
    public class CollectorOptions
    {
        [Option(longName: "Host", Required = true, HelpText = "Host address for the input receiver.")]
        public string Host { get; set; }
        
        [Option(longName: "Port", Required = true, HelpText = "Network port N and N+1 (UDP) to listen for incoming connecitons.")]
        public ushort Port { get; set; }
        
        [Option(longName: "PollingInterval", Default = (ushort)10, HelpText = "Polling interval for controller input")]
        public ushort PollingInterval { get; set; }
        
        [Option(longName: "NoConsole", Default = false, HelpText = "Do not render console")]
        public bool NoConsole { get; set; }
    }
}