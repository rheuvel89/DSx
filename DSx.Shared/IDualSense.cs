using DualSenseAPI;
using DualSenseAPI.State;

namespace DSx.Shared;

public interface IDualSense
{
    void Acquire();
    event Action<IDualSense> OnStatePolled;
    event Action<IDualSense, DualSenseInputStateButtonDelta> OnButtonStateChanged;
    DualSenseOutputState OutputState { get; }
    DualSenseAPI.State.DualSenseInputState InputState { get; }
    IoMode IoMode { get; }
    void BeginPolling(ushort pollingInterval);
}