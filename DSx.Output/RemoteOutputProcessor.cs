using System.Diagnostics;
using DSx.Mapping;
using DSx.Output.Shared;
using DXs.Common;
using Nefarius.ViGEm.Client;

namespace DSx.Output;

public class RemoteOutputProcessor : IOutputProcessor
{
    private readonly ConnectionManager _connectionManager;
    private readonly IList<IVirtualGamepad> _output;
    private readonly Stopwatch _timer;

    public RemoteOutputProcessor(string optionsReceiver, ushort optionsReceiverPort)
    {
        _connectionManager = new ConnectionManager(optionsReceiver, optionsReceiverPort);
        _output = new List<IVirtualGamepad>();
        _timer = new Stopwatch();
    }

    public IList<IVirtualGamepad> Output => _output;

    public async Task Initialize(Mapping.Mapping mapping)
    {
        _timer.Start();
        
        var controllers = Enumerable.Range(0, mapping.Count).Select<int, IVirtualGamepad>(i =>
        {
            return mapping[(byte)i] switch
            {
                ControllerType.DualShock => new SerializableDualShock4Controller(),
                ControllerType.XBox360 => new SerializableXbox360Controller()
            };
        });
        
        foreach (var controller in controllers) _output.Add(controller);
    }

    public void ProcessOutput()
    {
        using var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);
        writer.Write(_timer.ElapsedMilliseconds);
        writer.Write((ushort)_output.Count);
        for (var i = 0; i < _output.Count; i++)
        {
            writer.Write((ushort)i);
            writer.Serialize(_output[i]);
        }
        var bytes = stream.ToArray();
        _ = _connectionManager.Send(bytes);
    }

    public void Reset()
    {
        foreach (var controller in _output) controller.Disconnect();
        _output.Clear();
    }
}