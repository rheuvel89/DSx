using DSx.Shared;
using DualSenseAPI;
using DualSenseAPI.State;
using DualSenseInputState = DSx.Shared.DualSenseInputState;

namespace DSx.Input;

public class DualSenseWrapper : IDualSense
{
    private readonly DualSense _dualSense;

    public DualSenseWrapper(DualSense dualSense)
    {
        _dualSense = dualSense;
    }

    public void Acquire()
    {
        _dualSense.Acquire();
    }

    public event Action<IDualSense>? OnStatePolled
    {
        add
        {
            void OnDualSenseOnOnStatePolled(DualSense d) => value?.Invoke(new DualSenseWrapper(d));

            _dualSense.OnStatePolled += OnDualSenseOnOnStatePolled;
        }
        remove
        {
            void OnDualSenseOnOnStatePolled(DualSense d) => value?.Invoke(new DualSenseWrapper(d));

            _dualSense.OnStatePolled -= OnDualSenseOnOnStatePolled;
        }
    }

    public event Action<IDualSense, DualSenseInputStateButtonDelta>? OnButtonStateChanged
    {
        add
        {
            void OnDualSenseOnOnButtonStateChanged(DualSense d, DualSenseInputStateButtonDelta x) => value?.Invoke(new DualSenseWrapper(d), x);

            _dualSense.OnButtonStateChanged += OnDualSenseOnOnButtonStateChanged;
        }
        remove
        {
            void OnDualSenseOnOnButtonStateChanged(DualSense d, DualSenseInputStateButtonDelta x) => value?.Invoke(new DualSenseWrapper(d), x);
            
            _dualSense.OnButtonStateChanged -= OnDualSenseOnOnButtonStateChanged;
        }
    }

    public DualSenseOutputState OutputState => _dualSense.OutputState;

    public DualSenseAPI.State.DualSenseInputState InputState
    {
        get => _dualSense.InputState;
    }

    public IoMode IoMode
    {
        get => _dualSense.IoMode;
    }

    public void BeginPolling(ushort pollingInterval)
    {
        _dualSense.BeginPolling(pollingInterval);
    }
}