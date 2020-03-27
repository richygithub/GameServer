using SharedLib;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace UseLibuv
{
    interface IChannel
    {

    }



    public class Channel :IChannel, IDisposable
    {
        TcpHandle _handle;
        public TcpHandle Handle
        {
            get { return _handle; }
        }
        const int RECV_BUFF_LEN = 64*1024;
        const int SEND_BUFF_LEN = 64 * 1024;

        byte[] _recvBuff = new byte[RECV_BUFF_LEN];
        byte[] _sendBuff = new byte[SEND_BUFF_LEN];

        GCHandle recvGC;
        GCHandle sendGC;


        IRead _packetRead = new PacketRead();

        //int _id;

        //int Id { get { return _id; } }

        public int Id { get; set; }

        public Channel(TcpHandle handle)
        {
            _handle = handle;
            recvGC = GCHandle.Alloc(_recvBuff, GCHandleType.Pinned);
            sendGC = GCHandle.Alloc(_sendBuff, GCHandleType.Pinned);
        }
        ~Channel() => Dispose(false);
        public void Send(byte[] buf)
        {

        }
        public void Dispose()
        {
            //Console.WriteLine("")
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        bool _disposed = false;
        void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
                recvGC.Free();
                sendGC.Free();
            }
        }

        public void Recv()
        {


        }
        public void AllocRecv(out uv_buf_t buf)
        {

            buf = new uv_buf_t(recvGC.AddrOfPinnedObject(), RECV_BUFF_LEN);

        }
        public void ReadCB(int nread)
        {
            List<Packet> packets = new List<Packet>();
            _packetRead.process(_recvBuff, 0, nread, packets);
            foreach (var p in packets)
            {
                //string s = System.Text.Encoding.Default.GetString(p.body);
                //Console.WriteLine($"read packets:{s}");
            }
        }


    }


}
