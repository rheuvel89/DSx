using CommandLine;

namespace DSx.Host
{
    [Verb("Host")]
    public class HostOptions
    {
        [Option(longName: "Port", Required = true, HelpText = "Network port N (TCP) and N+1 (UDP) to listen for incoming connecitons.")]
        public int Port { get; set; }
        
        [Option(longName: "PollingInterval", Default = 10, HelpText = "Polling interval for controller input")]
        public uint PollingInterval { get; set; }
    }
}