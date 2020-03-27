using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using SharedLib;

namespace UseLibuv
{

/*
    public class Server
    {
        int _port;
        public Server(int port)
        {
            _port = port;

        }
        Dictionary<int, Channel> _clients = new Dictionary<int, Channel>();
        int _count = 0;

        static void AllocRecv(IntPtr handle, IntPtr suggested_size, out uv_buf_t buf)
        {

            TcpHandle h =  NativeHandle.GetDataFromHandle<TcpHandle>(handle);
            h.Channel.AllocRecv(out buf);
            //buf = new uv_buf_t();
            //buf = new uv_buf_t(recvGC.AddrOfPinnedObject(), RECV_BUFF_LEN);

        }
        static void ReadCB(IntPtr handle, IntPtr nread, ref uv_buf_t buf)
        {
            TcpHandle h = NativeHandle.GetDataFromHandle<TcpHandle>(handle);
            h.Channel.ReadCB(handle,nread);
        }

        Libuv.uv_alloc_cb Alloc = AllocRecv;
        Libuv.uv_read_cb Read = ReadCB;

        internal  void listenCB(IntPtr watcher, int status){
            Console.WriteLine("accept!!");
            
            IntPtr  clientHandle = Libuv.Allocate(Libuv.uv_handle_type.UV_TCP);
            //IntPtr  loop = 
            Libuv.uv_tcp_init(_loop, clientHandle);

            int ret = Libuv.uv_accept(watcher,clientHandle);
            if( ret == 0)
            {
                //Libuv.uv_re
                TcpHandle client = new TcpHandle(clientHandle);
                _count++;
                Channel c = new Channel(client, this, _count); ;
                _clients.Add(_count,c);

                Libuv.uv_read_start(clientHandle, Alloc, Read);



            }
            else
            {
                Console.WriteLine("accept error!!!");
 
                Libuv.uv_close(clientHandle, null);
                //free?
                Libuv.FreeMemory(clientHandle);

            }

        }
        public void Remove(int id)
        {
            Channel c;
            if( _clients.TryGetValue(id,out c))
            {
                _clients.Remove(id);
                var handle = c.Handle;
                handle.Dispose();


                Console.WriteLine($"remove!{id}");
            }

        }
        IntPtr _loop;
        public void Start()
        {
            _loop = Libuv.CreateLoop();
            Libuv.uv_loop_init(_loop);


            IPEndPoint ep = new IPEndPoint(IPAddress.Any, _port);

            

            IntPtr  tcpHandler = Libuv.Allocate(Libuv.uv_handle_type.UV_TCP);

            sockaddr addr = new sockaddr();
            Libuv.GetSocketAddress(ep,out addr);

            int r = Libuv.uv_tcp_init(_loop, tcpHandler);
            if(r != 0)
            {
                Console.WriteLine($"uv_tcp_init error:{r}");
            }

            r = Libuv.uv_tcp_bind(tcpHandler, ref addr, 0);
            if(r != 0)
            {
                Console.WriteLine($"uv_tcp_bind error:{r}");
            }


            r = Libuv.uv_listen(tcpHandler, 10, listenCB);
            if (r != 0)
            {
                Console.WriteLine($"uv_listen error:{r}");
            }

            Libuv.uv_run(_loop, Libuv.uv_run_mode.UV_RUN_DEFAULT);

        }




    }
    */
}
