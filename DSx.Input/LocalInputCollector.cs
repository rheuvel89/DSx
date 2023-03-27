using DSx.Mapping;
using DSx.Math;
using DualSenseAPI;
using DualSenseAPI.State;

namespace DSx.Input
{
    public class LocalInputCollector : InputCollector
    {
        private readonly ushort _pollingInterval;
        private DualSense _input;

        public LocalInputCollector(ushort pollingInterval)
        {
            _pollingInterval = pollingInterval;
        }
        
        public override async Task Start()
        {
            var controllers = DualSense.EnumerateControllers().ToList();
            _input = controllers.FirstOrDefault();
            if (_input == null) throw new Exception("No DualSense controllers connected");
            
            _input.Acquire();
            _input.OnStatePolled += DelegateInputReceived;
            _input.OnButtonStateChanged += DelegateButtonChanged; 
            _input.BeginPolling(_pollingInterval);
        }

        private void DelegateInputReceived(DualSense s)
        {
            OnInputReceived?.Invoke(s, s.InputState);
        }
        private void DelegateButtonChanged(DualSense s, DualSenseInputStateButtonDelta d)
        {
            OnButtonChanged?.Invoke(s, d);
        }

        public override event InputReceivedHandler? OnInputReceived;
        public override event ButtonChangedHandler? OnButtonChanged;
        public override void OnStateChanged(Feedback feedback)
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