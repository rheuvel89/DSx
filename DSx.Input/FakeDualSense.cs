using System.Reactive.Linq;
using DSx.Shared;
using DualSenseAPI;
using DualSenseAPI.State;
using BindingFlags = System.Reflection.BindingFlags;
using DualSenseInputState = DualSenseAPI.State.DualSenseInputState;

namespace DSx.Input;

public class FakeDualSense : IDualSense, IDisposable
{
    private IDisposable? _disposable;

    private readonly DualSenseOutputState _dualSenseOutputState;
    private readonly DualSenseInputState _dualSenseInputState;

    public FakeDualSense()
    {
        _disposable = null!;
        _dualSenseOutputState = new DualSenseOutputState();
        _dualSenseInputState = typeof(DualSenseInputState)
            .GetConstructor(BindingFlags.Instance|BindingFlags.NonPublic, [])?
            .Invoke([]) as DualSenseInputState ?? throw new InvalidOperationException("Failed to create DualSenseInputState");
    }

    public void Acquire() { }

    public event Action<IDualSense>? OnStatePolled;
    public event Action<IDualSense, DualSenseInputStateButtonDelta>? OnButtonStateChanged;
    public DualSenseOutputState OutputState => _dualSenseOutputState;
    public DualSenseInputState InputState => _dualSenseInputState;
    public IoMode IoMode => IoMode.USB;
    public void BeginPolling(ushort pollingInterval)
    {
        if (_disposable != null) throw new InvalidOperationException("Polling already started");
        _disposable = Observable.Interval(TimeSpan.FromMilliseconds(pollingInterval))
            .Subscribe(_ => OnStatePolled?.Invoke(this));
    }

    public void Dispose()
    {
        _disposable?.Dispose();
    }
}