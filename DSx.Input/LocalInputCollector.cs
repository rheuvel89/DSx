using DSx.Shared;
using DualSenseAPI;
using DualSenseAPI.State;
using DualSenseInputState = DSx.Shared.DualSenseInputState;

namespace DSx.Input
{
    public class LocalInputCollector : IInputCollector
    {
        private readonly ushort _pollingInterval;
        private IDualSense _input;

        public LocalInputCollector(ushort pollingInterval)
        {
            _pollingInterval = pollingInterval;
        }
        
        public async Task Start()
        {
            var controllers = DualSense.EnumerateControllers().ToList();
            var input = controllers.FirstOrDefault();
            if (input == null)
            {
                Console.WriteLine("Warning: No DualSense controllers connected");
                _input = new FakeDualSense();
            }
            else _input = new DualSenseWrapper(input);
            
            
            _input.Acquire();
            _input.OnStatePolled += DelegateInputReceived;
            _input.OnButtonStateChanged += DelegateButtonChanged; 
            _input.BeginPolling(_pollingInterval);
        }

        private void DelegateInputReceived(IDualSense ds)
        {
            var state = new DualSenseInputState(ds);
            OnInputReceived?.Invoke(state);
        }
        private void DelegateButtonChanged(IDualSense ds, DualSenseInputStateButtonDelta d)
        {
            OnButtonChanged?.Invoke(d);
        }

        public event InputReceivedHandler? OnInputReceived;
        public event ButtonChangedHandler? OnButtonChanged;
        public void OnStateChanged(Feedback feedback)
        {
            _input.OutputState.LeftRumble = feedback.Rumble.X;
            _input.OutputState.RightRumble = feedback.Rumble.Y;
            _input.OutputState.LightbarBehavior =
                feedback.Color.X == 0 && feedback.Color.Y == 0 && feedback.Color.Z == 0
                    ? LightbarBehavior.PulseBlue
                    : LightbarBehavior.CustomColor;
            _input.OutputState.LightbarColor = new LightbarColor { R = feedback.Color.X, G = feedback.Color.Y, B = feedback.Color.Z };
            _input.OutputState.MicLed = feedback.MicLed;
        }
    }
}