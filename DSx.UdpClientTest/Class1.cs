using System.Text;
using DXs.Common;

namespace DSx.UdpClientTest
{
    public class Program
    {
        public static async Task Main()
        {
            var socket = new ConnectionManager("127.0.0.1", 10801);
            var recvTask = socket.BeginReceiving();
            
            var sendTask = Task.Run(async () =>
            {
                var index = 0;
                while (true)
                {
                    byte[] data = Encoding.ASCII.GetBytes($"We send {index++} from the Client");
                    await socket.Send(data);
                    await Task.Delay(1000);
                }
            });

            Console.ReadLine();
        }
        
    }
}