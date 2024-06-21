using System.Diagnostics;
using System.Net;
using DSx.Input.Shared;
using DSx.Shared;
using DXs.Common;

namespace DSx.Input
{
    public class RemoteInputCollector : IInputCollector
    {
        private readonly ConnectionManager _connectionManager;
        private readonly Stopwatch _timer;
        private Task? _receiveTask;
        private long _ordering = 0;

        public RemoteInputCollector(ushort port)
        {
            _connectionManager = new ConnectionManager(port);
            _timer = new Stopwatch();
        }
        
        public async Task Start()
        {
            _connectionManager.OnPacketReceived += OnPackedReceived;
            _timer.Start();
            _receiveTask = _connectionManager.BeginReceiving();
        }

        private void OnPackedReceived(EndPoint sender, byte[] buffer, int length)
        {
            using var stream = new MemoryStream(buffer, 0, length);
            var reader = new BinaryReader(stream);
            var order = reader.ReadInt64();
            if (order < _ordering) return;
            Interlocked.Exchange(ref _ordering, order);
            var state = reader.DeserializeInputState();
            OnInputReceived?.Invoke(state);
        }

        public event InputReceivedHandler? OnInputReceived;
        public event ButtonChangedHandler? OnButtonChanged;
        public void OnStateChanged(Feedback feedback)
        {
            using var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            writer.Write(_timer.ElapsedMilliseconds);
            writer.Serialize(feedback);
            var bytes = stream.ToArray();
            _ = _connectionManager.Send(bytes);
        }
    }
}