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

        const int MAX_PAENDING_PACKET = 128;
        byte[] _recvBuff = new byte[RECV_BUFF_LEN];


        byte[] _sendBuff = new byte[SEND_BUFF_LEN];


        //Packet[] _pending = new Packet[MAX_PAENDING_PACKET];
        Queue<Packet> _pending = new Queue<Packet>(MAX_PAENDING_PACKET);
        int _pendingIdx = 0;

        int _sendOffset = 0;

        int _packetIdx = 0;

        public int CurPacketId => _packetIdx;

        bool _sending = false;

        GCHandle recvGC;
        GCHandle sendGC;

        OutputStream _ostream;
        public OutputStream OStream => _ostream;
        InputStream _istream;


        PacketRead _pr = new PacketRead();
        PacketWrite _pw = new PacketWrite();

        public delegate void OnReadCB(Channel c,byte[] buff,int nread);
        public event OnReadCB OnReadEvent;


        public delegate void OnWriteCB(Channel c, int status);
        public event OnWriteCB OnWriteEvent;

        //int _id;

        //int Id { get { return _id; } }

        public void IncPacketId()
        {
            _packetIdx++;
        }


        public int Id { get; set; }

        public Channel()
        {
            recvGC = GCHandle.Alloc(_recvBuff, GCHandleType.Pinned);
            sendGC = GCHandle.Alloc(_sendBuff, GCHandleType.Pinned);

            _ostream = new OutputStream(_sendBuff);
            _istream = new InputStream(_recvBuff);
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
        static int GetFullPacketSize(Packet p)
        {
            int len = p.seqId == 0 ? p.len : p.len + OutputStream.GetSize(p.seqId);
            return OutputStream.GetSize(len) + len;

        }

        public bool Send(Packet p)
        {
            //to do check length
            int len= p.len;
            if (p.seqId!=0)
            {
                len += OutputStream.GetSize(p.seqId); 
            }

            if(_ostream.EmptyLength < len || _sending )
            {
                if( _pending.Count == MAX_PAENDING_PACKET)
                {
                    // to do
                    return false;
                }
                _pending.Enqueue(p);
            }
            else
            {
                _ostream.WriteInt32(len);
                if( p.seqId!=0)
                {
                    _ostream.WriteUInt32(p.seqId);
                }
                _ostream.WriteRawByte(p.body,0,p.len);

                DoSend();
            }

            return true;

        }


        public bool Send()
        {
            if (!_sending)
            {
                DoSend();
                return true;
            }
            return false;
        }


        public void SendCB(int status)
        {
            _sending = false;
            _ostream.CleanUp();
            OnWriteEvent?.Invoke(this, status);
            while( _pending.Count > 0)
            {
                var p = _pending.Peek();
                if( GetFullPacketSize(p) > _ostream.EmptyLength)
                {
                    break;
                }
                if (p.seqId == 0)
                {
                    _ostream.WriteInt32(p.len);
                    _ostream.WriteRawByte(p.body,0,p.len);
                }
                else
                {
                    int len = p.len + OutputStream.GetSize(p.seqId);
                    _ostream.WriteInt32(len);
                    _ostream.WriteUInt32(p.seqId);
                    _ostream.WriteRawByte(p.body,0,p.len);
                }
                _pending.Dequeue();
            }
            DoSend();



        }

        void DoSend()
        {
            //需要优化
            if(_ostream.Length > 0)
            {

                WriteRequest req = new WriteRequest(uv_req_type.UV_WRITE,sendGC.AddrOfPinnedObject(), _ostream.Length);
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
