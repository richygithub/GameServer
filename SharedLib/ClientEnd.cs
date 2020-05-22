using Proto;
using SharedLib;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace UseLibuv 
{
    public class ClientEnd
    {
        //Dictionary<int, Channel> _servers = new Dictionary<int, Channel>();

        EventLoop _loop;
        Channel _channel;
        Tcp _handle;

        IDispatcher _dispatcher;
        PacketRead _packetRead = new PacketRead();
        IPEndPoint _ip;

        bool _connected;
        bool Connected => _connected;
        public OutputStream OutputStream => _channel.OStream;


        public delegate void OnConnectedCB(bool suc);
        public event OnConnectedCB OnConnectEvent;

        public ClientEnd(EventLoop loop, string host, int port)
        {
            _loop = loop;
            _channel = new Channel();
            _handle = new Tcp(_loop);
            _ip = new IPEndPoint(IPAddress.Parse(host), port);

            _handle.Channel = _channel;
            _channel.Handle = _handle;
            _connected = false;
        }

        void OnReadCB(Channel c, byte[] buff, int nread)
        {
            if (nread < 0)
            {
                //remove
                //RemoveChannel(c);
                //Console.WriteLine($"remove channel:{c.Id}");
                _connected = false;
            }
            else
            {
                List<Packet> packets = new List<Packet>();
                _packetRead.process(buff, 0, nread, packets);
                Console.WriteLine($"Recv:{nread} at channel:{c.Id}");

                _dispatcher.Process(_loop, c, packets);

            }


        }
        void OnConnected(ConnectRequest request)
        {
            if( request.Error != null)
            {
                //
                Console.WriteLine($"Connect error!{request.Error}");

                request.Dispose();
                OnConnectEvent?.Invoke(false);

            }
            else
            {

                _connected = true;
                _channel.OnReadEvent += OnReadCB;
                //_channel.OnWriteEvent +=
                OnConnectEvent?.Invoke(true);
            }

        }
        public void Connect()
        {
            ConnectRequest cr = new ConnectRequest(_handle.Handle,_ip);
            cr.ConnectedEvent += OnConnected;

        }

    }

}
