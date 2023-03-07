using System.Text;
using DXs.Common;

namespace DSx.UdpHostTest
{
    public class Program
    {
        public static async Task Main()
        {
            var socket = new ConnectionManager(10801);
            var recvTask = socket.Receive();

            var sendTask = Task.Run(async () =>
            {
                var index = 0;
                while (true)
                {
                    byte[] data = Encoding.ASCII.GetBytes($"We send {index++} from the Host");
                    await socket.Send(data);
                    await Task.Delay(1000);
                }
            });

            Console.ReadLine();
        }
    }
}