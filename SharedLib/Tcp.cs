using System;
using System.Collections.Generic;
using System.Text;

namespace UseLibuv
{
    sealed public unsafe class Tcp : TcpHandle
    {
        public Tcp(EventLoop loop) : base(loop)
        {
        }

        public Channel Channel{get;set;}

        static public uv_alloc_cb AllocCB = OnAllocCB;
        static public uv_read_cb ReadCB = OnReadCB;


        void OnReadCB(IntPtr num)
        {
            //Channel.ReadCB( nread);
            int nread = (int)num;
            Channel.ReadCB(nread);

            /*
            if (nread < 0)
            {
                //remove
                //_eventLoop.re
                ChannelRemoveEvent?.Invoke(Channel);
                Dispose();
            }
            else
            {
            }
            */

        }

        void Send(byte[] buff)
        {
            //Libuv.uv_write()


        }


        static void OnAllocCB(IntPtr handle, IntPtr suggested_size, out uv_buf_t buf)
        {

            Tcp h = GetDataFromHandle<Tcp>(handle);
            h.Channel.AllocRecv(out buf);
            //buf = new uv_buf_t();
            //buf = new uv_buf_t(recvGC.AddrOfPinnedObject(), RECV_BUFF_LEN);

        }
        static void OnReadCB(IntPtr handle, IntPtr nread, ref uv_buf_t buf)
        {
            Tcp h = NativeHandle.GetDataFromHandle<Tcp>(handle);
            h.OnReadCB(nread);
        }


        protected override void OnClosed()
        {
            base.OnClosed();
            Channel.Dispose();
        }
    }
}
