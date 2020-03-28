using System;
using System.Collections.Generic;
using System.Text;

namespace UseLibuv 
{
    unsafe public class Timer:NativeHandle
    {
        EventLoop _eventLoop;
        readonly Action<object> _callback;
        public Timer(EventLoop loop, Action<object> cb,object state)
        {
            _eventLoop = loop;
            _callback = cb;

            _handle = Libuv.Allocate(Libuv.uv_handle_type.UV_TIMER);

            Libuv.uv_timer_init(_eventLoop.Loop, _handle);



        }
    }
}
