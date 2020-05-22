using Proto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace UseLibuv
{
    public partial class OutputStream
    {
        internal static readonly Encoding Utf8Encoding = Encoding.UTF8;
        byte[] _buff;
        int _len;
        int _pos;


        public byte[] Buff => _buff;

        public int Length => _pos;
        public int EmptyLength => _len - _pos;
        public OutputStream(byte[] buff)
        {
            _buff = buff;
            _len = _buff.Length;
            _pos = 0;
        }

        public void CleanUp()
        {
            _pos = 0;
        }

        /*
                [Conditional("Debug")]
                public void Write<T>(T value)
                {

                }
                */
        public void WriteUInt32(uint value)
        {
            WriteVarint32(value);
            //WriteFixed32( (uint)value);
        }
        public void WriteInt32(int value)
        {
            WriteVarint32((uint)value );
            //WriteFixed32( (uint)value);
        }
        public void WriteInt64(long value)
        {
            WriteRawVarint64((ulong)value);
        }


        public void WriteBool(bool value)
        {
            WriteByte(value ? (byte)1 : (byte)0);
        }

        
        public void WriteRawByte(byte[] bytes,int offset,int len)
        {
            Buffer.BlockCopy( bytes,offset,_buff,_pos,len );
        }




        public void WritePacketHead(byte type,uint len)
        {
            ulong head = Packet.GetPacketHead(type, len);
            WriteRawVarint64(head);
        }
        
        public void WriteService(byte serverType,uint serviceId )
        {
            uint service = Packet.GetService(serverType, serviceId);
            WriteUInt32(service);
        }


        public void WriteString(string value)
        {
            // Optimise the case where we have enough space to write
            // the string directly to the buffer, which should be common.
            int length = Utf8Encoding.GetByteCount(value);
            WriteLength(length);
            if (length == value.Length) // Must be all ASCII...
            {
                for (int i = 0; i < length; i++)
                {
                    _buff[_pos + i] = (byte)value[i];
                }
            }
            else
            {
                Utf8Encoding.GetBytes(value, 0, value.Length, _buff, _pos);
            }
            _pos += length;
        }
        public void WriteLength(int length)
        {
            WriteVarint32((uint)length);
            //WriteFixed32((uint)length);
        }

        internal void WriteByte(byte value)
        {
            _buff[_pos++] = value;
        }
        internal void WriteFixed32(uint value)
        {
            _buff[_pos++] = ((byte)value);
            _buff[_pos++] = ((byte)(value >> 8));
            _buff[_pos++] = ((byte)(value >> 16));
            _buff[_pos++] = ((byte)(value >> 24));
        }

        internal bool WriteVarint32(uint value)
        {
            // Optimize for the common case of a single byte value
            if (value < 128 && _pos < _len )
            {
                _buff[_pos++] = (byte)value;
                return true;
            }

            while (value > 127 && _pos < _len)
            {
                _buff[_pos++] = (byte)((value & 0x7F) | 0x80);
                value >>= 7;
            }
            if( value > 127 || _pos >= _len )
            {
                return false;
            }

            _buff[_pos++] = (byte)value;

            return true;
        }

        internal bool WriteRawVarint64(ulong value)
        {
            while (value > 127 && _pos < _len)
            {
                _buff[_pos++] = (byte)((value & 0x7F) | 0x80);
                value >>= 7;
            }
            while (value > 127 && _pos < _len )
            {
                _buff[_pos++] = (byte)((value & 0x7F) | 0x80);
                value >>= 7;
            }
            if (value > 127 || _pos >= _len)
            {
                return false;
            }
            _buff[_pos++] = (byte)value;

            return true;
        }

    }
}
