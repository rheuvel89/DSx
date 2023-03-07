using System.Net;
using DXs.Common;

namespace DSx.Input
{
    public class RemoteInputCollector : InputCollector
    {
        private readonly ConnectionManager _connectionManager;
        private Task? _receiveTask;

        public RemoteInputCollector(ushort port)
        {
            _connectionManager = new ConnectionManager(port);
        }
        
        public override async Task Start()
        {
            _connectionManager.OnPacketReceived += (sender, buffer, length) => Console.WriteLine($"{sender as IPEndPoint}: {length}");
            _receiveTask = _connectionManager.BeginReceiving();
        }

        public override event InputReceivedHandler? OnInputReceived;
        public override event ButtonChangedHandler? OnButtonChanged;
    }
}