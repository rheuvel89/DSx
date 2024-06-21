using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DSx.Input;
using DSx.Input.Shared;
using DualSenseAPI.State;
using DXs.Common;
using DualSenseInputState = DSx.Shared.DualSenseInputState;

namespace DSx.Collector
{
    public class Collector : IApplication
    {
        private readonly IInputCollector _inputCollector;
        private readonly ConnectionManager _connectionManager;
        private readonly DSx.Console.Console _console;
        private readonly Stopwatch _timer;
        private Task _receiveTask;
        private long _ordering = 0;

        public Collector(CollectorOptions options)
        {
            _inputCollector = new LocalInputCollector(options.PollingInterval);
            _connectionManager = new ConnectionManager(options.Host, options.Port);
            _console = new Console.Console(null, options.NoConsole);
            _timer = new Stopwatch();
        }

        public async Task Initialize()
        {
            _inputCollector.OnInputReceived += OnInputReceived;
            _inputCollector.OnButtonChanged += OnButtonChanged;

            _console.OnCommandReceived += OnCommandReceived;
        }

        public async Task Start()
        {
            await Initialize();

            _timer.Start();

            await _inputCollector.Start();

            _connectionManager.OnPacketReceived += OnPacketReceived;
            _receiveTask = _connectionManager.BeginReceiving();
            
            await _console.Attach();
        }

        private void OnPacketReceived(EndPoint sender, byte[] buffer, int length)
        {
            using var stream = new MemoryStream(buffer, 0, length);
            var reader = new BinaryReader(stream);
            var order = reader.ReadInt64();
            if (order < _ordering) return;
            Interlocked.Exchange(ref _ordering, order);
            var feedback = reader.DeserializeFeedback();
            _inputCollector.OnStateChanged(feedback);
        }

        private void OnInputReceived(DualSenseInputState inputState)
        {
            using var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            writer.Write(_timer.ElapsedMilliseconds);
            writer.Serialize(inputState);
            var bytes = stream.ToArray();
            _ = _connectionManager.Send(bytes);
        }

        private void OnButtonChanged(DualSenseInputStateButtonDelta change) { }

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