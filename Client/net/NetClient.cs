using SharedLib;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UseLibuv;
using Proto;
namespace Client
{
    public partial class NetClient
    {
        static NetClient _instance = new NetClient();

        static public NetClient Instance => _instance;
        private NetClient()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _ostream = new OutputStream(_sendBuf);
        }

        static NetClient()
        {

        }

        const int MAX_RECVBUF_LEN = 1024;
        const int MAX_SENDBUF_LEN = 1024;


        byte[] _recvBuf = new byte[MAX_RECVBUF_LEN];
        byte[] _sendBuf = new byte[MAX_SENDBUF_LEN];


        PacketRead _pr = new PacketRead();
        List<Packet> _packets = new List<Packet>();

        void DoReceive(int count)
        {
            //other thread
            _pr.process(_recvBuf, 0, count,_packets);
            foreach(var  p in _packets)
            {
                Process(p);
            }
            _packets.Clear();


        }
        void ReceivedEvent(object sender, SocketAsyncEventArgs e)
        {
            if(e.SocketError == SocketError.Success)
            {
                Console.WriteLine($"recv successful! in thread:{Thread.CurrentThread.ManagedThreadId},count:{e.BytesTransferred},{e.Offset}");

                DoReceive(e.BytesTransferred);
                StartReceive();

            }
            else
            {
                 Console.WriteLine($"recv error! in thread:{Thread.CurrentThread.ManagedThreadId}");
 
            }

        }

        void StartReceive()
        {
            SocketAsyncEventArgs recvEventArg = new SocketAsyncEventArgs();
            recvEventArg.SetBuffer(_recvBuf);
            recvEventArg.Completed += ReceivedEvent;
            var ret = _socket.ReceiveAsync(recvEventArg);
            if (!ret)
            {
                Console.WriteLine($"false!{recvEventArg.BytesTransferred}");
                ReceivedEvent(this, recvEventArg);
            }
        }


        void ConnectedEvent(object sender, SocketAsyncEventArgs e)
        {
            if( e.SocketError == SocketError.Success)
            {
                Send();
                StartReceive();

                //Console.WriteLine($"connect successful! in thread:{Thread.CurrentThread.ManagedThreadId},recv:{ret},{e.BytesTransferred}");
            }
            else
            {
                Console.WriteLine($"connect error!{e.SocketError}"); 
            }
        }

        Socket _socket;
        public void Connect( string host,int port )
        {

            Console.WriteLine($"Connect in thread:{Thread.CurrentThread.ManagedThreadId}");
            EventHandler<SocketAsyncEventArgs> s;
            SocketAsyncEventArgs eventArg = new SocketAsyncEventArgs();
            eventArg.Completed += ConnectedEvent;
            eventArg.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(host),port );
            _socket.ConnectAsync(eventArg);
        }

    }
}
