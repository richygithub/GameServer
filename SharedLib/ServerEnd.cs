using SharedLib;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace UseLibuv 
{
    public class ServerEnd
    {
        Dictionary<int, Channel> _clients = new Dictionary<int, Channel>();
        EventLoop _loop;

        //Dictionary<int,

        Dispatcher _dispatcher;
        TcpListen _tcpHandle;
        IPEndPoint _ip;
        public ServerEnd(EventLoop loop, Dispatcher dispatcher, int port)
        {
            _loop = loop;
            //IPEndPoint ep = new IPEndPoint(IPAddress.Any, _port);
            _tcpHandle = new TcpListen(_loop);

            _tcpHandle.ChannelAddEvent += AddChannel;

            _ip = new IPEndPoint(IPAddress.Any, port);
            _tcpHandle.Bind(_ip);
            _dispatcher = dispatcher;

            //_dispatcher = new Dispatcher(_loop);
        }

        public void Start()
        {
            _tcpHandle.Listen();
        }

        IRead _packetRead = new PacketRead();

        delegate void ReadEvent(Channel c, byte[] buff, int nread);

        void OnReadCB(Channel c, byte[] buff, int nread)
        {
            if (nread < 0)
            {
                //remove
                RemoveChannel(c);
                //Console.WriteLine($"remove channel:{c.Id}");

            }
            else
            {
                List<Packet> packets = new List<Packet>();
                _packetRead.process(buff, 0, nread, packets);
                Console.WriteLine($"Recv:{nread} at channel:{c.Id}");

                _dispatcher.Process(_loop, c, packets);

            }


        }

        void OnWriteCB(Channel c, int status)
        {
            //need to remove
            Console.WriteLine($"Write Over.Channel:{c.Id},status:{status}");

        }

        int _count = 0;
        public void AddChannel(Channel channel)
        {
            channel.Id = _count;
            //channel.Loop = this;
            //channel.
            channel.OnReadEvent += OnReadCB;
            channel.OnWriteEvent += OnWriteCB;

            _clients.Add(_count, channel);
            _count++;
        }
        public void RemoveChannel(Channel channel)
        {
            _clients.Remove(channel.Id);
        }


    }
}
