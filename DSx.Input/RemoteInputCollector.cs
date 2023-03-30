using System.Diagnostics;
using System.Net;
using DSx.Mapping;
using DSx.Math;
using DualSenseAPI;
using DualSenseAPI.State;
using DXs.Common;

namespace DSx.Input
{
    public class RemoteInputCollector : InputCollector
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
        
        public override async Task Start()
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
            var ds = reader.Deserialize<DualSenseState>();
            var state = reader.Deserialize<DualSenseInputState>();
            OnInputReceived?.Invoke(ds, state);
        }

        public override event InputReceivedHandler? OnInputReceived;
        public override event ButtonChangedHandler? OnButtonChanged;
        public override void OnStateChanged(Feedback feedback)
        {
            using var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            writer.Write(_timer.ElapsedMilliseconds);
            writer.Serialize(feedback);
            var bytes = stream.ToArray();
            _connectionManager.Send(bytes);
        }
    }
}