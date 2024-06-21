using CommandLine;

namespace DSx.Receiver;

[Verb("Receiver")]
public class ReceiverOptions
{
        
    [Option(longName: "Port", Required = true, HelpText = "Network port N and N+1 (UDP) to listen for incoming connecitons.")]
    public ushort Port { get; set; }
        
    [Option(longName: "NoConsole", Default = false, HelpText = "Do not render console")]
    public bool NoConsole { get; set; }
}