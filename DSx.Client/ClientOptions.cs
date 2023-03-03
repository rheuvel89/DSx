using CommandLine;

namespace DSx.Client
{
    [Verb("Client")]
    public class ClientOptions
    {
        [Option(longName: "Port", Default = null, HelpText = "Network port N (TCP) and N+1 (UDP) for controller input. Defaults to input from local controller.")]
        public ushort? Port { get; set; }
        
        [Option(longName: "Server", Default = null, HelpText = "Server address for controller input. Defaults to input from local controller.")]
        public string Server { get; set; }
        
        [Option(longName: "PollingInterval", Default = 10u, HelpText = "Polling interval for controller input")]
        public uint PollingInterval { get; set; }
    }
}