using System;
using System.Net.Sockets;
using System.Net;
using SharedLib;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UseLibuv;
using System.Collections.Concurrent;
using Tutorial;

namespace Client
{



    class Program 
    {
        unsafe static void Main(string[] args)
        {
            string host = "121.36.16.144";
            int port = 11240;
            host = "192.168.25.199";

            int i = -1;
            long t = (uint)i<<2;

            //ulong ut = i;

            //Console.WriteLine($"{(int)t},{(byte)ut}");
            
            NetClient.Instance.Connect(host, port);
            Area.PlayerHandler.GetUserNum("abcdefgh");

            Console.ReadLine();

            //host = "8.8.8.8";
            //port = 9999;
/*
            EventLoop loop = new EventLoop();

            ClientEnd c = new ClientEnd(loop, host, port);

            c.Connect();
            loop.Run();
            */

        }

    }
    /*
     public class Handler
     {
         public int GetNum(int count)
         {
             return count;
         }

         public Task<string> GetName()
         {
             var t = Task.Run<string>(() =>
             {
                 Thread.Sleep(1000);

                 return "abcde";
             });
             return t;  
         }

         TaskCompletionSource<int> t = new TaskCompletionSource<int>();
         public void Set(int i)
         {
             t.SetResult(i);
         }
         public void Cancel()
         {

             //t.SetResult(i);
             t.SetCanceled();

         }


         public Task<int> GetAge()
         {

             //Task<int> tt = new Task<int>( ()=> { return 1; });


             //return tt;

             return t.Task;

         }

     }



     public class Client
     {

         EventLoop _eventLoop;

         ConcurrentQueue<Action> _jobs = new ConcurrentQueue<Action>();


         Async _jobAsync;

         public EventLoop Loop => _eventLoop;
         public Client()
         {
             _eventLoop = new EventLoop();

             _jobAsync = new Async(_eventLoop, doJob, null);


         }

         public void Start(long interval)
         {
             _eventLoop.Run(interval);

         }

         void doJob(object o)
         {
             Action job;
             if(_jobs.TryDequeue(out job))
             {
                 job();
             }
         }

         public void AddAction()
         {

         }




     }

     public struct Server
     {

         public int i;
         public int j;
         public Server(int a1,int a2)
         {
             i = a1;
             j = a2;

         }

     }



     class Program
     {

         const string ipstr = "127.0.0.1";
         const int port = 11240;


         static async void Test(Handler h)
         {

             var s= await h.GetName();
             Console.WriteLine($"name is :{s}");

             int i = await h.GetAge();
             Console.WriteLine($"age is :{i}");



         }



         static void Pro(out Server s)
         {



             s = new Server(1, 2);
             getMemory(s);



         }
         static public string getMemory(object o) // 获取引用类型的内存地址方法    
         {
             GCHandle h = GCHandle.Alloc(o, GCHandleType.WeakTrackResurrection);

             IntPtr addr = GCHandle.ToIntPtr(h);

             string s="0x" + addr.ToString("X");
             Console.WriteLine(s);
             return s;
         }
        unsafe static void Main(string[] args)
         {
             string host = "121.36.16.144";
             int port = 11240;
             host = "8.8.8.8";
             port = 9999;

             EventLoop loop = new EventLoop();

             ClientEnd c = new ClientEnd(loop, host, port);

             c.Connect();
             loop.Run();
         }
     }
     */
}
