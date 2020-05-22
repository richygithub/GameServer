using System;
using System.Collections.Generic;
using System.Text;

namespace UseLibuv 
{
    public class WriteRequest : NativeRequest
    {

        uv_buf_t _buf;

        public delegate void OnWrite(int status);
        public event OnWrite OnWriteEvent;


        static public uv_watcher_cb OnWriteCB = OnWriteCallback;

        public WriteRequest(uv_req_type requestType,IntPtr buf, int len ) : base(requestType,len)
        {
            _buf = new uv_buf_t(buf,len);

        }

        public void Write(IntPtr dst)
        {
            Libuv.uv_write(Handle, dst, ref _buf, 1, OnWriteCB);
        }

        /*
        protected override void Dispose(bool disposing)
        {
           base.Dispose(disposing);
        }
        */
        void OnWriteCallback(int status)
        {
            OnWriteEvent?.Invoke(status);

            Dispose();

        }
        static void OnWriteCallback(IntPtr handle, int status)
        {
            var request = NativeHandle.GetDataFromHandle<WriteRequest>(handle);
            request.OnWriteCallback(status);
        }

    }
}
