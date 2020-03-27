using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace UseLibuv
{
    abstract public unsafe class TcpHandle:NativeHandle
    {
        protected EventLoop _eventLoop;
        public TcpHandle(EventLoop loop) : base()
        {
            _handle = Libuv.Allocate(Libuv.uv_handle_type.UV_TCP);
            _eventLoop = loop;

            Libuv.uv_tcp_init(loop.Loop, _handle);

            GCHandle gcHandle = GCHandle.Alloc(this, GCHandleType.Normal);
            ((uv_handle_t*)_handle)->data = GCHandle.ToIntPtr(gcHandle); 
        }

        protected override void OnClosed()
        {
            base.OnClosed();
        }

    }
}
