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
    }
}