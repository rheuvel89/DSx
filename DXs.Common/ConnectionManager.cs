using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DXs.Common
{
    public delegate void PacketReceivedHandler(EndPoint sender, byte[] buffer, int length);

    public class ConnectionManager
    {
        private readonly int _port;
        private Socket _incomingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private Socket _outGoingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private const int BufferSize = 8 * 1024;
        private readonly SemaphoreSlim _mutex = new SemaphoreSlim(0, 2);

        public class State
        {
            public byte[] buffer = new byte[BufferSize];
        }

        public ConnectionManager(int port)
        {
            _port = port;
            _incomingSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            _incomingSocket.Bind(new IPEndPoint(IPAddress.Any, port));
        }
        
        public event PacketReceivedHandler? OnPacketReceived;

        public ConnectionManager(string address, int port) : this(port + 1)
        {
            _outGoingSocket.Connect(IPAddress.Parse(address), port);
        }

        public async Task Send(byte[] data)
        {
            if (!_outGoingSocket.Connected) return;
            _outGoingSocket.BeginSend(data, 0, data.Length, SocketFlags.None, x =>
            {
                var bytes = _outGoingSocket.EndSend(x);
                Console.WriteLine("SEND: {0}", bytes);
            }, null);
        }

        public async Task Receive()
        {
            while (true)
            {
                EndPoint endPoint = new IPEndPoint(IPAddress.Any, _port);
                var buffer = new byte[BufferSize];

                _incomingSocket.BeginReceiveFrom(buffer, 0, BufferSize, SocketFlags.None, ref endPoint, x =>
                {
                    try
                    {
                        int bytes = _incomingSocket.EndReceiveFrom(x, ref endPoint);
                        if (!_outGoingSocket.Connected) _outGoingSocket.Connect(new IPEndPoint((endPoint as IPEndPoint).Address, _port + 1));
                        Console.WriteLine("RECV: {0}: {1}, {2}", endPoint, bytes, Encoding.ASCII.GetString(buffer, 0, bytes));
                        OnPacketReceived?.Invoke(endPoint, buffer, bytes);
                    }
                    finally
                    {
                        _mutex.Release();
                    }
                }, null);
                await _mutex.WaitAsync();
            }
        }
    }
}