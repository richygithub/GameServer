using System;
using System.Net.Sockets;
using System.Net;
using SharedLib;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Client
{
    class Program
    {

        const string ipstr = "127.0.0.1";
        const int port = 11240;

        static List<byte[]> _mems = new List<byte[]>();

        static List<IntPtr> _ptrs = new List<IntPtr>();


        static void Clear()
        {
            return;
            foreach(var ptr in _ptrs) {
                Marshal.FreeCoTaskMem(ptr);
            }
            _ptrs.Clear();

        }
        static void TestGC()
        {
            var mem = new byte[1024*64];
            //var gcHandle = GCHandle.Alloc(mem);
            var ptr= Marshal.AllocCoTaskMem(1024 * 64);

            _mems.Add(mem);
            //_ptrs.Add(ptr);

            Marshal.FreeCoTaskMem(ptr);
            if ( _mems.Count == 100)
            {
                _mems.Clear();
                Clear();
            }


        }
        static void Main(string[] args)
        {
            int count = 0;

            while (true)
            {
                Thread.Sleep(100);
                count++;
                if (count < 100)
                {
                    TestGC();
                }else if(count == 100)
                {
                    _mems.Clear();
                    Clear();
                }
            }
            //DispatcherTimer 

            /*
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = IPAddress.Parse(ipstr);

            var addr = new IPEndPoint(ip, port);
            //s.Bind(new IPEndPoint(ip, port));
            s.Connect(addr);

            Console.WriteLine("connect");

            PacketWrite pw = new PacketWrite();

            s.Send(pw.Write("it is a client"));

            while (true)
            {
                string str = Console.ReadLine();
                s.Send(pw.Write(str));


            }

    */


        }
    }
}
