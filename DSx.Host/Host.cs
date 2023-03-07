using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using DSx.Input;
using DualSenseAPI;
using DualSenseAPI.State;
using DXs.Common;
using Nefarius.ViGEm.Client;

namespace DSx.Host
{
    public class Host : IApplication
    {
        private readonly ushort _serverPort;
        private readonly InputCollector _inputCollector;
        private readonly IList<IVirtualGamepad> _output;
        private readonly DSx.Console.Console _console;
        private readonly Stopwatch _timer;
        private TcpClient _tcpClient = null;
        private UdpClient _udpClient = null;

        public Host(HostOptions options)
        {
            _serverPort = options.Port;
            _inputCollector = new LocalInputCollector(options.PollingInterval);
            _output = new List<IVirtualGamepad>();
            _console = new Console.Console(_output, options.NoConsole);
            _timer = new Stopwatch();
        }

        public async Task Initialize(object mapping)
        {
            _inputCollector.OnInputReceived += OnInputReceived;
            _inputCollector.OnButtonChanged += OnButtonChanged;

            _console.OnCommandReceived += OnCommandReceived;
        }

        public async Task Start()
        {
            await Initialize(null);

            _timer.Start();

            await _inputCollector.Start();

            var listener = TcpListener.Create(_serverPort);
            listener.Start();
            while (true)
            {
                System.Console.WriteLine($"Waiting for connection on port {_serverPort}");
                _tcpClient = await listener.AcceptTcpClientAsync();

                // var udpSocket = new UDPSocket();
                // udpSocket.Client((_tcpClient.Client.RemoteEndPoint as IPEndPoint).Address.MapToIPv4().ToString(), _serverPort + 1);
                // udpSocket.Send("Succes!!!1");
                // await Task.Delay(1000);
                // udpSocket.Send("Succes!!!2");
                // await Task.Delay(1000);
                // udpSocket.Send("Succes!!!3");
                // await Task.Delay(1000);
                // udpSocket.Send("Succes!!!4");
                // await Task.Delay(1000);
                // udpSocket.Send("Succes!!!5");
                // await _console.Attach();
            }
            
        }

        private void OnInputReceived(DualSense ds)
        {
            var send = _udpClient?.Send(new byte[] { 1 });
        }

        private void OnButtonChanged(DualSense ds, DualSenseInputStateButtonDelta change)
        {

        }

        private string? OnCommandReceived(string command, string[] arguments)
        {
            command = command.ToLower();
            return command switch
            {
                _ => $"Command {command} not recognized"
            };
        }
    }
}