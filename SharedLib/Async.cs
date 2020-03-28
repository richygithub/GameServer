using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace UseLibuv
{
    unsafe public class Async : NativeHandle
    {
        EventLoop _eventLoop;

        readonly Action<object> _callback;
        readonly object _state;
        public Async(EventLoop loop,Action<object> cb,object state)
        {
            var handle=Libuv.Allocate(Libuv.uv_handle_type.UV_ASYNC);

            _handle = handle;

            _eventLoop = loop;

            _callback = cb;
            _state = state;


            Libuv.uv_async_init(loop.Loop, _handle, OnWorkCallback);

            GCHandle gcHandle = GCHandle.Alloc(this, GCHandleType.Normal);
            ((uv_handle_t*)_handle)->data = GCHandle.ToIntPtr(gcHandle);

        }
        public void Send()
        {
            Libuv.uv_async_send(this.Handle);
        }
        public void OnWorkCallback()
        {
            _callback?.Invoke(_state);


        }
        static void OnWorkCallback(IntPtr handle)
        {
            var workHandle = GetDataFromHandle<Async>(handle);
            workHandle?.OnWorkCallback();
        }

    }
}
