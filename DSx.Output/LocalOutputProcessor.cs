using DSx.Mapping;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;

namespace DSx.Output;

public class LocalOutputProcessor : IOutputProcessor
{
    private readonly ViGEmClient _manager;
    private readonly IList<IVirtualGamepad> _output;

    public LocalOutputProcessor()
    {
        _manager = new ViGEmClient();
        _output = new List<IVirtualGamepad>();
    }

    public IList<IVirtualGamepad> Output => _output;

    public async Task Initialize(Mapping.Mapping mapping)
    {
        var controllers = Enumerable.Range(0, mapping.Count).Select<int, IVirtualGamepad>(i =>
        {
            return mapping[(byte)i] switch
            {
                ControllerType.DualShock => CreateXbox360Controller((ushort)i),
                ControllerType.XBox360 => CreateDualShock4Controller((ushort)i),
                _ => throw new ArgumentOutOfRangeException()
            };
        });

        foreach (var controller in controllers)
        {
            _output.Add(controller);
            controller.Connect();
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }

    public IXbox360Controller CreateXbox360Controller(ushort id)
    {
        var controller = _manager.CreateXbox360Controller(0x7331, (ushort)id);
        _output.Add(controller);
        controller.Connect();
        return controller;
    }
    
    public IDualShock4Controller CreateDualShock4Controller(ushort id)
    {
        var controller = _manager.CreateDualShock4Controller(0x7331, (ushort)id);
        _output.Add(controller);
        controller.Connect();
        return controller;
    }
    
    public void ProcessOutput()
    {
        foreach (var controller in _output) controller.SubmitReport();
    }
    
    public void Reset()
    {
        foreach (var controller in _output) controller.Disconnect();
        _output.Clear();
    }
}