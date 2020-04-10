using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SharedLib
{

    public class Packet
    {
        public int len;
        public uint seqId;
        public byte packetType;
        public byte serverType;
        public byte[] body;
    }

    public interface IRead
    {
        void process(byte[] buff,int offset,int len, List<Packet> packets);
    }

    public class PacketRead :IRead
    {
        enum State
        {
            HEAD = 0,
            BODY
        }

        public const int HeadLen = 4;

        int _bodyLen = 0;
        int _bufOffset = 0;

        byte[] _bodyBuf;
        State _status = State.HEAD;

        int _packetLen = 0;
        int _shift = 0;

        int getPackLen()
        {
            return _packetLen;
        }
        void processHead(byte[] buff,int offset,int limit, List<Packet> packets)
        {
            int readLen = 0;
            bool finish = false;
            
            do
            {
                byte b = buff[offset++];
                _packetLen |= (b & 0x7F) << _shift;

                if (b < 0x80)
                {
                    finish = true;
                    break;
                }
                readLen++;
            }
            while (_shift < 32 && offset< limit);

            if( _shift == 32 || finish)
            {
                _status = State.BODY;
                _bodyLen = _packetLen ;
                _bodyBuf = new byte[_bodyLen];
                _bufOffset = 0;
                _shift = 0;

                if( offset < limit)
                {
                    processBody(buff, offset  ,limit , packets);
                }
            }

        }
        void processBody(byte[] buff,int offset,int limit, List<Packet> packets)
        {
            int len = limit - offset;
            if( len >= _bodyLen - _bufOffset )
            {
                int readLen = _bodyLen - _bufOffset;

                Buffer.BlockCopy( buff, offset, _bodyBuf , _bufOffset, readLen );
                //finish
                Packet p = new Packet() { body = _bodyBuf };
                packets.Add(p);

                _status = State.HEAD;
                //int left = len - offset - _leftlen;
                if( len > readLen )
                {
                    processHead( buff, offset + readLen ,limit , packets );
                }

            }
            else
            {
                Buffer.BlockCopy( buff, offset, _bodyBuf , _bufOffset, len );
                _bufOffset += len;
            }

        }

        public void process(byte[] buff,int offset,int limit,List<Packet> packets )
        {
            if(_status == State.HEAD)
            {
                processHead(buff, offset, limit, packets);
            }
            else
            {
                processBody(buff, offset, limit, packets);
            }
        }
    }
}
