using System;
using System.Collections.Generic;
using System.Text;

namespace UseLibuv
{
    public class OutputStream
    {
        internal static readonly Encoding Utf8Encoding = Encoding.UTF8;
        byte[] _buff;
        int _len;
        int _pos;
        public OutputStream(byte[] buff)
        {
            _buff = buff;
            _len = _buff.Length;
            _pos = 0;
        }

        public void WriteInt32(int value)
        {
            WriteFixed32( (uint)value);
        }
        public void WriteBool(bool value)
        {
            WriteByte(value ? (byte)1 : (byte)0);
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
                    buffer[position + i] = (byte)value[i];
                }
            }
            else
            {
                Utf8Encoding.GetBytes(value, 0, value.Length, buffer, position);
            }
            position += length;
        }
        public void WriteLength(int length)
        {
            WriteFixed32((uint)length);
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

        internal bool WriteRawVarint32(uint value)
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

    }
}
