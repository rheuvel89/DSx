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
        private readonly InputCollector _inputCollector;
        private readonly ConnectionManager _connectionManager;
        private readonly DSx.Console.Console _console;
        private readonly Stopwatch _timer;
        private Task _receiveTask;

        public Host(HostOptions options)
        {
            _inputCollector = new LocalInputCollector(options.PollingInterval);
            _connectionManager = new ConnectionManager(options.Server, options.Port);
            _console = new Console.Console(null, options.NoConsole);
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

            _connectionManager.OnPacketReceived += (sender, buffer, length) => System.Console.WriteLine($"{sender as IPEndPoint}: {length}");
            _receiveTask = _connectionManager.BeginReceiving();
            
            await _console.Attach();
        }

        private void OnInputReceived(DualSense ds)
        {
            var send = _connectionManager.Send(new byte[] { 1 });
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