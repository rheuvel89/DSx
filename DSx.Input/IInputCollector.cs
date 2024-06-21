using DSx.Shared;

namespace DSx.Input
{
    public interface IInputCollector
    {
         Task Start();
         event InputReceivedHandler? OnInputReceived;   
         event ButtonChangedHandler? OnButtonChanged;
         void OnStateChanged(Feedback feedback);
    }
}