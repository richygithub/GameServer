using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace UseLibuv
{
    sealed public unsafe class TcpListen: TcpHandle
    {
        public TcpListen(EventLoop loop) : base(loop)
        {
        }
        static uv_watcher_cb ListenCB = OnListenCB;
        static void OnListenCB(IntPtr watcher, int status)
        {
            //Console.WriteLine("accept!!");

            var tcpListen = GetDataFromHandle<TcpListen>(watcher);
            tcpListen.OnListenCB(status);

        }
        public delegate void ChannelAddHandler(Channel channel);
        public event ChannelAddHandler ChannelAddEvent;


        void OnListenCB(int status)
        {

            Tcp tcp = new Tcp(_eventLoop);
            int ret = Libuv.uv_accept(_handle, tcp.Handle);
            if (ret == 0)
            {
                //Libuv.uv_re
                Channel c = new Channel( );
                tcp.Channel = c;
                c.Handle = tcp;

                ChannelAddEvent?.Invoke(c);

                Libuv.uv_read_start(tcp.Handle, Tcp.AllocCB, Tcp.ReadCB);
            }
            else
            {
                Console.WriteLine("accept error!!!");

                tcp.Dispose();

  
            }

        }
        public void Listen()
        {
            int r = Libuv.uv_listen(_handle, 10, ListenCB);
            if (r != 0)
            {
                Console.WriteLine($"uv_listen error:{r}");
            }

        }
        public void Bind(IPEndPoint ip)
        {


            sockaddr addr = new sockaddr();
            Libuv.GetSocketAddress(ip, out addr);

            int r = Libuv.uv_tcp_init(_eventLoop.Loop, _handle);
            if (r != 0)
            {
                Console.WriteLine($"uv_tcp_init error:{r}");
            }

            r = Libuv.uv_tcp_bind(_handle, ref addr, 0);
            if (r != 0)
            {
                Console.WriteLine($"uv_tcp_bind error:{r}");
            }



            //Libuv.uv_run(_loop, Libuv.uv_run_mode.UV_RUN_DEFAULT);
        }
    }
}
