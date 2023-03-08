using CommandLine;

namespace DSx.Host
{
    [Verb("Host")]
    public class HostOptions
    {
        [Option(longName: "Server", Required = true, HelpText = "Server address for the input receiver.")]
        public string Server { get; set; }
        
        [Option(longName: "Port", Required = true, HelpText = "Network port N and N+1 (UDP) to listen for incoming connecitons.")]
        public ushort Port { get; set; }
        
        [Option(longName: "PollingInterval", Default = (ushort)10, HelpText = "Polling interval for controller input")]
        public ushort PollingInterval { get; set; }
        
        [Option(longName: "NoConsole", Default = false, HelpText = "Do not render console")]
        public bool NoConsole { get; set; }
    }
}