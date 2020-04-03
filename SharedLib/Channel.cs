using Google.Protobuf;
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



    public unsafe class Channel : IChannel, IDisposable
    {

        public Tcp Handle
        {
            get;set;
        }

        const int RECV_BUFF_LEN = 64*1024;
        const int SEND_BUFF_LEN = 64 * 1024;

        byte[] _recvBuff = new byte[RECV_BUFF_LEN];


        byte[] _sendBuff = new byte[SEND_BUFF_LEN];
        int _sendOffset = 0;

        int _packetIdx = 0;

        bool _sending = false;

        GCHandle recvGC;
        GCHandle sendGC;

        PacketRead _pr = new PacketRead();
        PacketWrite _pw = new PacketWrite();

        public delegate void OnReadCB(Channel c,byte[] buff,int nread);
        public event OnReadCB OnReadEvent;


        public delegate void OnWriteCB(Channel c, int status);
        public event OnWriteCB OnWriteEvent;

        //int _id;

        //int Id { get { return _id; } }

        public void BeginPacket()
        {
            _packetIdx = _sendOffset;

        }
        public void EndPacket()
        {
            _packetIdx = _sendOffset;
        }
        public void AddInt(int value)
        {

        }

        public void AddMessage(IMessage msg)
        {


        }

        public int Id { get; set; }

        public Channel()
        {
            recvGC = GCHandle.Alloc(_recvBuff, GCHandleType.Pinned);
            sendGC = GCHandle.Alloc(_sendBuff, GCHandleType.Pinned);
        }
        ~Channel() => Dispose(false);
        int _packetId = 0;
        public void Send(string s, PacketWrite p)
        {
            //to do ,缓存
            if( !_sending)
            {
                int len = p.Write(_packetId,s, _sendBuff,_sendOffset);
                _sendOffset += len;
                DoSend();
            }

        }
        public void Send(byte[] s)
        {

        }

        public void SendCB(int status)
        {
            _sending = false;
            _sendOffset = 0;

            OnWriteEvent?.Invoke(this, status);
        }

        void DoSend()
        {
            //需要优化
            if(_sendOffset > 0)
            {

                WriteRequest req = new WriteRequest(uv_req_type.UV_WRITE,sendGC.AddrOfPinnedObject(),_sendOffset);
                req.Write(Handle.Handle);
                req.OnWriteEvent += SendCB;

                _sending = true;

                //Libuv.uv_write(req.Handle, Handle.Handle, ref req.uv_buf, 1, OnWriteCB);
            }

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
            OnReadEvent?.Invoke(this,_recvBuff,nread);
            /*
            List<Packet> packets = new List<Packet>();
            _packetRead.process(_recvBuff, 0, nread, packets);
            foreach (var p in packets)
            {
                //string s = System.Text.Encoding.Default.GetString(p.body);
                //Console.WriteLine($"read packets:{s}");
            }
            */
        }



    }


}
