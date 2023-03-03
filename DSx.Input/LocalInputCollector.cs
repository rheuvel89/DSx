using DualSenseAPI;

namespace DSx.Input
{
    public class LocalInputCollector : InputCollector
    {
        private readonly uint _pollingInterval;
        private DualSense _input;

        public LocalInputCollector(uint pollingInterval)
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
            _input.BeginPolling(_pollingInterval);
        }

        private void DelegateInputReceived(DualSense s)
        {
            OnInputReceived?.Invoke(s);
        }

        public override event InputReceivedHandler? OnInputReceived;
    }
}