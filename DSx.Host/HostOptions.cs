using CommandLine;

namespace DSx.Host
{
    [Verb("Host")]
    public class HostOptions
    {
        [Option(longName: "Port", Required = true, HelpText = "Network port N (TCP) and N+1 (UDP) to listen for incoming connecitons.")]
        public ushort Port { get; set; }
        
        [Option(longName: "PollingInterval", Default = 10u, Required = false, HelpText = "Polling interval for controller input")]
        public ushort PollingInterval { get; set; }
        
        [Option(longName: "NoConsole", Default = false, Required = false, HelpText = "Do not render console")]
        public bool NoConsole { get; set; }
    }
}