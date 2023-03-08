using System.Diagnostics;
using System.Net;
using DSx.Math;
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
            var state = reader.Deserialize<DualSenseInputState>();
            OnInputReceived?.Invoke(null, state);
        }

        public override event InputReceivedHandler? OnInputReceived;
        public override event ButtonChangedHandler? OnButtonChanged;
        public override void OnStateChanged(Vector<float, float> rumble)
        {
            _connectionManager.Send(rumble.Serialize(_timer.ElapsedMilliseconds));
        }
    }
}