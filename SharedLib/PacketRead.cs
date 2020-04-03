using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SharedLib
{

    public class Packet
    {
        public int id;
        public int type;
        public byte[] head;
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

        public const int HeadLen = 8;

        int _bodyLen = 0;
        int _bufOffset = 0;

        byte[] _headBuf = new byte[HeadLen];
        byte[] _bodyBuf;

        State _status = State.HEAD;


        public string ReadString(Packet p)
        {
            return System.Text.Encoding.Default.GetString(p.body);

        }
        int getPackLen()
        {
            return _headBuf[0] | _headBuf[1] << 8 | _headBuf[2] <<16 | _headBuf[3] <<24;
        }
        int ReadInt(byte[] buff, int offset)
        {
            return buff[offset+0] | buff[offset+1] << 8 | buff[offset+2] << 16 | buff[offset+3] << 24;
        }
        void processHead(byte[] buff,int offset,int len, List<Packet> packets)
        { 
            if (len >= HeadLen - _bufOffset)
            {
                int readLen = HeadLen - _bufOffset;


                Buffer.BlockCopy(buff, offset , _headBuf, _bufOffset, readLen );


                _bodyLen = getPackLen();

                Debug.Assert(_bodyLen < 16*1024 && _bodyLen>=0 );

                _status = State.BODY;
                _bodyBuf = new byte[_bodyLen];
                _bufOffset = 0;

                if(len > readLen )
                {
                    processBody(buff, offset + readLen, len -readLen ,packets);
                }

            }
            else
            {
                Buffer.BlockCopy(buff, offset, _headBuf, _bufOffset, len );
                _bufOffset += len ;


            }


        }
        void processBody(byte[] buff,int offset,int len, List<Packet> packets)
        {
            if( len >= _bodyLen - _bufOffset )
            {
                int readLen = _bodyLen - _bufOffset;


                Buffer.BlockCopy( buff, offset, _bodyBuf , _bufOffset, readLen );
                //finish
                Packet p = new Packet() { head = _headBuf, body = _bodyBuf };
                packets.Add(p);


                _status = State.HEAD;
                _bufOffset = 0;
                _headBuf = new byte[HeadLen];
                //int left = len - offset - _leftlen;
                if( len > readLen )
                {
                    processHead( buff, offset + readLen ,len - readLen , packets );
                }

            }
            else
            {
                Buffer.BlockCopy( buff, offset, _bodyBuf , _bufOffset, len );
                _bufOffset += len;
            }

        }

        public void process(byte[] buff,int offset,int len,List<Packet> packets )
        {

            if(_status == State.HEAD)
            {
                processHead(buff, offset,len,packets);
            }
            else
            {
                processBody(buff, offset,len,packets);
            }
        }
    }
}
