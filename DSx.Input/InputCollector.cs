namespace DSx.Input
{
    public abstract class InputCollector
    {
        public abstract Task Start();
        public abstract event InputReceivedHandler? OnInputReceived;   
        public abstract event ButtonChangedHandler? OnButtonChanged;   
    }
}