using System.Reactive.Linq;
using Nefarius.ViGEm.Client;
using SystemConsole = System.Console;

namespace DSx.Console
{
    public class Console
    {
        private readonly object _lock = new object();
        private readonly IList<IVirtualGamepad> _output;

        public Console(IList<IVirtualGamepad> output)
        {
            _output = output;
        }
        
        public event CommandReceivedHandler? OnCommandReceived;
        
        public async Task Attach()
        {
            SystemConsole.Clear();
            
            using var refreshObservable = Observable.Interval(TimeSpan.FromMilliseconds(100)).Subscribe(OnRefresh);
            
            while (true)
            {
                lock (_lock)
                {
                    SystemConsole.SetCursorPosition(0, SystemConsole.WindowHeight - 8);
                    SystemConsole.WriteLine(string.Empty.PadRight(SystemConsole.WindowWidth - 1));
                    SystemConsole.SetCursorPosition(0, SystemConsole.WindowHeight - 7);
                    SystemConsole.WriteLine(string.Empty.PadRight(SystemConsole.WindowWidth - 1));
                    SystemConsole.WriteLine(new string(Enumerable.Repeat('=', SystemConsole.WindowWidth - 1).ToArray()));
                    SystemConsole.SetCursorPosition(0, SystemConsole.WindowHeight - 1);
                    SystemConsole.Write("> ".PadRight(SystemConsole.WindowWidth - 1));
                    SystemConsole.SetCursorPosition(3, SystemConsole.WindowHeight - 1);
                }
                var line = SystemConsole.ReadLine();
                
                if (line == "exit") return;
                if (line == null) continue;
                
                var split = line.Split();
                var command = split.First();
                var arguments = split.Skip(1).ToArray();
                
                var error = OnCommandReceived?.Invoke(command, arguments);
                
                if (error != null) await SystemConsole.Error.WriteLineAsync(error);
            }
        }

        private void OnRefresh(long timestamp)
        {
            lock (_lock)
            {
                var cursorVisible = SystemConsole.CursorVisible;
                SystemConsole.CursorVisible = false;
                ConsoleFunctions.PrintState(_output[1]);
                SystemConsole.CursorVisible = cursorVisible;
            }
        }
    }
}