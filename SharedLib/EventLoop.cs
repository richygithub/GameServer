using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace UseLibuv
{
    public class ClientEnd
    {
        Dictionary<int, Channel> _servers = new Dictionary<int, Channel>();

        EventLoop _loop;

        
        public ClientEnd(EventLoop loop)
        {
            _loop = loop;

        }


        public void Connect(string host, int port)
        {

        }

    }
    public class ServerEnd
    {
        Dictionary<int, Channel> _clients = new Dictionary<int, Channel>();
        EventLoop _loop;


        TcpListen _tcpHandle;
        IPEndPoint _ip;
        public ServerEnd(EventLoop loop,int port)
        {
            _loop = loop;
            //IPEndPoint ep = new IPEndPoint(IPAddress.Any, _port);
            _tcpHandle = new TcpListen(_loop);

            _tcpHandle.ChannelAddEvent      += AddChannel;
            _tcpHandle.ChannelRemoveEvent += RemoveChannel;

            _ip = new IPEndPoint(IPAddress.Any, port);
            _tcpHandle.Bind(_ip);
        }

        public void Start()
        {
            _tcpHandle.Listen();
        }
        
        int _count = 0;
        public void AddChannel(Channel channel)
        {
            channel.Id = _count;
            _clients.Add(_count,channel);
            _count++;
        }
        public void RemoveChannel(Channel channel)
        {
            _clients.Remove(channel.Id);
        }


    }

    unsafe public class EventLoop : IDisposable
    {
        IntPtr _loop;
        public IntPtr Loop => _loop;
        public EventLoop()
        {
            _loop = Libuv.CreateLoop();
            Libuv.uv_loop_init(_loop);
            _async = new Async(this, doAsyncJob, null);

        }
        ConcurrentQueue<Action> _asyncJobs= new ConcurrentQueue<Action>();

        void doAsyncJob(object obj=null)
        {
            while(_asyncJobs.Count > 0)
            {
                Action job;

                if (_asyncJobs.TryDequeue(out job))
                {
                    job();
                }
            }


        }
        void Update(long dt)
        {
            Console.WriteLine($"Upate {dt}");

        }

        Timer _timer;
        Async _async;
        public void Run(long interval=0)
        {
            if( interval>0)
            {
                _timer = new Timer(this, (obj) =>
                {
                    long dt = Libuv.uv_now(_loop);
                    Update(dt);
                },null);

                _timer.Start(interval,interval);

            }

            Libuv.uv_run(_loop, Libuv.uv_run_mode.UV_RUN_DEFAULT);
        }
        public void AddAsyncJob(Action action)
        {
            _asyncJobs.Enqueue(action);
            _async.Send();

        }




        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }
                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。
                Libuv.FreeMemory(_loop);
                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        ~EventLoop()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(false);
        }

        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            GC.SuppressFinalize(this);
        }
        #endregion


    }

}
