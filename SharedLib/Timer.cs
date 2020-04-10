using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace UseLibuv 
{
    unsafe public class Timer:NativeHandle
    {
        EventLoop _eventLoop;
        object _state;
        readonly Action<object> _callback;

        static readonly Libuv.uv_work_cb WorkCallback = OnWorkCallback;
        public Timer(EventLoop loop, Action<object> cb,object state)
        {
            _eventLoop = loop;
            _callback = cb;
            _state = state;

            _handle = Libuv.Allocate(uv_handle_type.UV_TIMER);

            Libuv.uv_timer_init(_eventLoop.Loop, _handle);


            GCHandle gcHandle = GCHandle.Alloc(this, GCHandleType.Normal);
            ((uv_handle_t*)_handle)->data = GCHandle.ToIntPtr(gcHandle);


        }
        static void OnWorkCallback(IntPtr handle)
        {
            var workHandle = GetDataFromHandle<Timer>(handle);
            workHandle?.OnWorkCallback();
        }
        void OnWorkCallback()
        {
            _callback?.Invoke(this);
        }

        public void Start(long interval,long repeated)
        {
            Libuv.uv_timer_start(_handle, WorkCallback, interval, repeated);
        }

    }
}
