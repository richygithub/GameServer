using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace UseLibuv
{
    public class ConnectRequest :NativeRequest
    {
        protected static readonly uv_watcher_cb WatcherCallback = OnWatcherCallback;
        private OperationException error;
        public delegate void ConnectCBHandler(ConnectRequest request);
        public event ConnectCBHandler ConnectedEvent;
        public ConnectRequest(IntPtr handle, IPEndPoint remoteEndPoint) : base(uv_req_type.UV_CONNECT, 0)
        {
            Libuv.GetSocketAddress(remoteEndPoint, out sockaddr addr);
            int result = Libuv.uv_tcp_connect(
                this.Handle,
                handle,
                ref addr,
                WatcherCallback);
            //ThrowIfError(result);
            if( result <0)
            {
                error = Libuv.CreateError((uv_err_code)result);
                OnWatcherCallback();
            }


        }

        protected  void OnWatcherCallback()
        {
            ConnectedEvent?.Invoke(this);
        }

        internal OperationException Error => this.error;

        static void OnWatcherCallback(IntPtr handle, int status)
        {
            var request = GetDataFromHandle<ConnectRequest>(handle);
            if (status < 0)
            {
                request.error =Libuv.CreateError((uv_err_code)status);
            }
            request.OnWatcherCallback();
        }
    }
}
