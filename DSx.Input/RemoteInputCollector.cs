using System.Net;
using System.Net.Sockets;
using DXs.Common;

namespace DSx.Input
{
    public class RemoteInputCollector : InputCollector
    {
        private readonly string _server;
        private readonly ushort _port;

        public RemoteInputCollector(string server, ushort port)
        {
            _server = server;
            _port = port;
        }
        
        public override async Task Start()
        {
            System.Console.WriteLine($"Create TCP {_server} : {_port}");
            using var tcpClient = new TcpClient(_server, _port);
            System.Console.WriteLine($"Connected TCP {tcpClient.Connected}");
            System.Console.WriteLine($"Create UDP {_server} : {_port + 1}");
            // var udpSocket = new UDPSocket();
            // udpSocket.Server(_server, _port + 1);
            // await Task.Delay(10000);
            // while (tcpClient.Connected)
            // {
            //     System.Console.WriteLine($"Send UDP");
            //     udpSocket.Send("Yay!");
            //     await Task.Delay(1000);
            // }
        }

        public override event InputReceivedHandler? OnInputReceived;
        public override event ButtonChangedHandler? OnButtonChanged;
    }
}