using System.Net;
using DSx.Output;
using DSx.Output.Shared;
using DXs.Common;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;

namespace DSx.Receiver;

public class Receiver : IApplication
{
    private readonly LocalOutputProcessor _outputProcessor;
    private readonly ConnectionManager _connectionManager;
    private readonly DSx.Console.Console _console;
    private Task _receiveTask;
    private long _ordering = 0;
    
    public Receiver(ReceiverOptions options)
    {
        _outputProcessor = new LocalOutputProcessor();
        _connectionManager = new ConnectionManager(options.Port);
        _console = new DSx.Console.Console(null, options.NoConsole);
    }

    public async Task Initialize()
    {
        _console.OnCommandReceived += OnCommandReceived;
    }
    
    public async Task Start()
    {
        await Initialize();
        
        _connectionManager.OnPacketReceived += OnPacketReceived;
        _receiveTask = _connectionManager.BeginReceiving();
        await _console.Attach();
        await _receiveTask;
    }

    private void OnPacketReceived(EndPoint sender, byte[] buffer, int length)
    {
        using var stream = new MemoryStream(buffer, 0, length);
        var reader = new BinaryReader(stream);
        var order = reader.ReadInt64();
        if (order < _ordering) return;
        Interlocked.Exchange(ref _ordering, order);
        var count = reader.ReadUInt16();
        for (var i = 0; i < count; i++)
        {
            var index = reader.ReadUInt16();
            var deserializedController = reader.DeserializeVirtualGamepad();
            if (index > _outputProcessor.Output.Count) continue;
            if (index == _outputProcessor.Output.Count) CreateController(deserializedController, index);
            _outputProcessor.Output[index].Update(deserializedController);
        }
        _outputProcessor.ProcessOutput();
    }
    
    private void CreateController(IVirtualGamepad deserializedController, ushort id)
    {
        switch (deserializedController)
        {
            case IXbox360Controller: _outputProcessor.CreateXbox360Controller(id); break;
            case IDualShock4Controller: _outputProcessor.CreateDualShock4Controller(id); break;
        }
    }
    
    
    
    private string? OnCommandReceived(string command, string[] arguments)
    {
        command = command.ToLower();
        return command switch
        {
            "reset" => Reset(),  
            _ => $"Command {command} not recognized"
        };
    }

    private string? Reset()
    {
        _outputProcessor.Reset();
        Interlocked.Exchange(ref _ordering, 0);
        return null;
    }
}