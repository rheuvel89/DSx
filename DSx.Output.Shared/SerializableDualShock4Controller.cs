using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;

namespace DSx.Output;

public class SerializableDualShock4Controller : IDualShock4Controller
{
    public IDictionary<int, bool> _buttonStates = new Dictionary<int, bool>();
    public IDictionary<int, short> _axisValues = new Dictionary<int, short>();
    public IDictionary<int, byte> _sliderValues = new Dictionary<int, byte>();
    
    public void Connect() { }

    public void Disconnect() { }

    public void SetButtonState(int index, bool pressed) => _buttonStates[index] = pressed;

    public void SetAxisValue(int index, short value) => _axisValues[index] = value;

    public void SetSliderValue(int index, byte value) => _sliderValues[index] = value;

    public void ResetReport()
    {
        _buttonStates.Clear();
        _axisValues.Clear();
        _sliderValues.Clear();
    }

    public void SubmitReport() { }

    public int ButtonCount { get; }
    public int AxisCount { get; }
    public int SliderCount { get; }
    public bool AutoSubmitReport { get; set; }
    public void Dispose()
    {
    }

    public void SetButtonState(DualShock4Button button, bool pressed) => SetButtonState(button.Id, pressed); 

    public void SetDPadDirection(DualShock4DPadDirection direction) => SetButtonState(direction.Id, true);

    public void SetAxisValue(DualShock4Axis axis, byte value) => SetAxisValue(axis.Id, value);

    public void SetSliderValue(DualShock4Slider slider, byte value) => SetSliderValue(slider.Id, value);

    public void SetButtonsFull(ushort buttons)
    {
        throw new NotImplementedException();
    }

    public void SetSpecialButtonsFull(byte buttons)
    {
        throw new NotImplementedException();
    }

    public void SubmitRawReport(byte[] buffer) { }

    public IEnumerable<byte> AwaitRawOutputReport() { return new byte[0];}

    public IEnumerable<byte> AwaitRawOutputReport(int timeout, out bool timedOut)
    {
        timedOut = false; 
        return new byte[0];
    }

    public ref byte LeftTrigger => throw new NotImplementedException();

    public ref byte RightTrigger => throw new NotImplementedException();

    public ref byte LeftThumbX => throw new NotImplementedException();

    public ref byte LeftThumbY => throw new NotImplementedException();

    public ref byte RightThumbX => throw new NotImplementedException();

    public ref byte RightThumbY => throw new NotImplementedException();

    public event DualShock4FeedbackReceivedEventHandler? FeedbackReceived;
}