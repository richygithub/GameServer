using System;
using System.Collections.Generic;
using System.Text;

namespace UseLibuv
{
    public class ConnectRequest :NativeRequest
    {
        protected static readonly uv_watcher_cb WatcherCallback = OnWatcherCallback;


        protected ConnectRequest() : base(uv_req_type.UV_CONNECT, 0)
        {
        }

        protected  void OnWatcherCallback()
        {

        }

        //internal OperationException Error => this.error;

        static void OnWatcherCallback(IntPtr handle, int status)
        {
            /*
            var request = GetTarget<ConnectRequest>(handle);
            if (status < 0)
            {
                request.error = NativeMethods.CreateError((uv_err_code)status);
            }
            request.OnWatcherCallback();
            */
        }
    }
}
