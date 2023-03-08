using System.Net;
using DualSenseAPI.State;
using DXs.Common;

namespace DSx.Input
{
    public class RemoteInputCollector : InputCollector
    {
        private readonly ConnectionManager _connectionManager;
        private Task? _receiveTask;
        private long _ordering = 0;

        public RemoteInputCollector(ushort port)
        {
            _connectionManager = new ConnectionManager(port);
        }
        
        public override async Task Start()
        {
            _connectionManager.OnPacketReceived += OnPackedReceived;
            _receiveTask = _connectionManager.BeginReceiving();
        }

        private void OnPackedReceived(EndPoint sender, byte[] buffer, int length)
        {
            using var stream = new MemoryStream(buffer, 0, length);
            var reader = new BinaryReader(stream);
            var order = reader.ReadInt64();
            if (order < _ordering) return;
            Interlocked.Exchange(ref _ordering, order);
            var state = reader.Deserialize<DualSenseInputState>();
            OnInputReceived?.Invoke(null, state);
        }

        public override event InputReceivedHandler? OnInputReceived;
        public override event ButtonChangedHandler? OnButtonChanged;
    }
}