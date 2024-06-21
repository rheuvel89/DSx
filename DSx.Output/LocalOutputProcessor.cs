using DSx.Mapping;
using DSx.Shared;
using Nefarius.ViGEm.Client;

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
    public async Task Initialize(Mapping.Mapping mapping)
    {
        var controllers = Enumerable.Range(0, mapping.Count).Select<int, IVirtualGamepad>(i =>
        {
            return mapping[(byte)i] switch
            {
                ControllerType.DualShock => _manager.CreateDualShock4Controller(0x7331, (ushort)i),
                ControllerType.XBox360 => _manager.CreateXbox360Controller(0x7331, (ushort)i),
            };
        });

        foreach (var controller in controllers)
        {
            _output.Add(controller);
            controller.Connect();
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }

    public Feedback Map(Mapping.Mapping mapping, DualSenseInputState inputState)
    {
        return mapping.Map(inputState, _output);
    }

    public void ProcessOutput()
    {
        foreach (var controller in _output) controller.SubmitReport();
    }
}