namespace DSx.Input
{
    public class RemoteInputCollector : InputCollector
    {
        private readonly string _server;
        private readonly ushort _port;

        public RemoteInputCollector(string server, ushort port)
        {
            _server = server;
            _port = port;
        }
        
        public override Task<bool> Start()
        {
            throw new NotImplementedException();
        }

        public override event InputReceivedHandler? OnInputReceived;
    }
}