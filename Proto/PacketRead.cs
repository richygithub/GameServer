using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Proto 
{
    public enum PacketType
    {
        HEART_BEAT = 0,
        REQ = 1,
        NOTIFY = 2,
        RESP = 3,
        PUSH = 4,
    }
    public class Packet
    {
        public int len;
        public uint seqId;
        public byte packetType;
        public uint serviceId;
        public byte[] body;

        public static byte GetPacketType(ulong packetHead)
        {
            return (byte)packetHead;
        }
        public static uint GetPacketLen(ulong packetHead)
        {
            return (uint)(packetHead >> 8);
        }

        public static ulong GetPacketHead(byte type, uint len)
        {
            return type | (ulong)len << 8;
        }

        public static byte GetServiceServerType(uint service)
        {
            return (byte)(service & 0xFF);
        }
        public static uint GetServiceId(uint service)
        {
            return service >> 8;
        }

        public static uint GetService(byte serverType,uint serviceId )
        {
            return serviceId << 8 | serverType;
        }




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


        uint _bodyLen = 0;
        int _bufOffset = 0;

        byte[] _bodyBuf;
        State _status = State.HEAD;

        uint _packetLen = 0;
        int _shift = 0;

        ulong _packetHead= 0;


        void ResetHeadState()
        {
            _status = State.HEAD;
            _shift = 0;
            _packetHead = 0;
            _packetLen = 0;
        }


         void processHead(byte[] buff,int offset,int limit, List<Packet> packets)
        {
            int readLen = 0;
            bool finish = false;
            
            do
            {
                byte b = buff[offset++];
                _packetHead |= (ulong)(b & 0x7F) << _shift;

                if (b < 0x80)
                {
                    finish = true;
                    break;
                }
                _shift += 7;
                readLen++;
            }
            while (_shift < 64 && offset< limit);

            if( _shift == 64 || finish)
            {

                if( Packet.GetPacketType(_packetHead) == (byte)PacketType.HEART_BEAT)
                {
                    Packet p = new Packet();
                    p.packetType = (byte)PacketType.HEART_BEAT;
                    packets.Add(p);

                    ResetHeadState();

                    if (offset < limit)
                    {
                        processHead(buff, offset, limit, packets);
                    }
                }
                else
                {
                    _packetLen = Packet.GetPacketLen(_packetHead);
                    _status = State.BODY;
                    _bodyLen = _packetLen;
                    _bodyBuf = new byte[_bodyLen];
                    _bufOffset = 0;
                    _shift = 0;

                    if (offset < limit)
                    {
                        processBody(buff, offset, limit, packets);
                    }
                }
            }

        }
        void processBody(byte[] buff,int offset,int limit, List<Packet> packets)
        {
            int len = limit - offset;
            if( len >= _bodyLen - _bufOffset )
            {
                int readLen = (int)_bodyLen - _bufOffset;

                Buffer.BlockCopy( buff, offset, _bodyBuf , _bufOffset, readLen );
                //finish
                Packet p = new Packet() { body = _bodyBuf };
                p.packetType = Packet.GetPacketType(_packetHead);
                packets.Add(p);

                ResetHeadState();
                //_status = State.HEAD;
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
