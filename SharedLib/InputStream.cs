using System;
using System.Collections.Generic;
using System.Text;

namespace UseLibuv 
{
    public partial class InputStream
    {
        internal static readonly Encoding Utf8Encoding = Encoding.UTF8;

        byte[] _buff;
        int _len;
        int _pos;

        public InputStream()
        {
            CleanUp();
        }
        public InputStream(byte[] buff)
        {
            _buff = buff;
            _len = _buff.Length;
            _pos = 0;
        }
        public void SetUp(byte[] buff,int len)
        {
            _buff = buff;
            _len = len;
            _pos = 0; 
        }
        public void CleanUp()
        {
            _buff = null;
            _len = 0;
            _pos = 0;
        }

        public int PeekInt32(int offset=-1)
        {
            offset = offset < 0 ? _pos : offset;

            int tmp = _buff[offset++];
            if (tmp < 128)
            {
                return (int)tmp;
            }
            int result = tmp & 0x7f;
            if ((tmp = _buff[offset++]) < 128)
            {
                result |= tmp << 7;
            }
            else
            {
                result |= (tmp & 0x7f) << 7;
                if ((tmp = _buff[offset++]) < 128)
                {
                    result |= tmp << 14;
                }
                else
                {
                    result |= (tmp & 0x7f) << 14;
                    if ((tmp = _buff[offset++]) < 128)
                    {
                        result |= tmp << 21;
                    }
                    else
                    {
                        result |= (tmp & 0x7f) << 21;
                        result |= (tmp = _buff[offset++]) << 28;
                    }
                }
            }
            return result;
        }

        public int ReadInt32()
        {
            return (int)ReadVarint32();
        }
        public uint ReadUInt32()
        {
            return ReadVarint32();
        }
        public long ReadInt64()
        {
            return (long)ReadRawVarint64();
        }
        public int ReadLength()
        {
            return (int)ReadVarint32();
        }
        public string ReadString()
        {
            int length = ReadLength();
            // No need to read any data for an empty string.
            if (length == 0)
            {
                return "";
            }
            if (length <= _len - _pos && length > 0)
            {
                // Fast path:  We already have the bytes in a contiguous buffer, so
                //   just copy directly from it.
                String result = Utf8Encoding.GetString(_buff, _pos, length);
                _pos += length;
                return result;
            }
            throw InvalidBufferException.MalformedString();
            //return CodedOutputStream.Utf8Encoding.GetString(ReadRawBytes(length), 0, length);
        }
        public bool ReadBool()
        {
            return ReadByte() != 0;
        }
        public byte ReadByte()
        {
            return _buff[_pos++];
        }

        uint ReadVarint32()
        {

            int tmp = _buff[_pos++];
            if (tmp < 128)
            {
                return (uint)tmp;
            }
            int result = tmp & 0x7f;
            if ((tmp = _buff[_pos++]) < 128)
            {
                result |= tmp << 7;
            }
            else
            {
                result |= (tmp & 0x7f) << 7;
                if ((tmp = _buff[_pos++]) < 128)
                {
                    result |= tmp << 14;
                }
                else
                {
                    result |= (tmp & 0x7f) << 14;
                    if ((tmp = _buff[_pos++]) < 128)
                    {
                        result |= tmp << 21;
                    }
                    else
                    {
                        result |= (tmp & 0x7f) << 21;
                        result |= (tmp = _buff[_pos++]) << 28;
                    }
                }
            }
            return (uint)result;
        }
        internal ulong ReadRawVarint64()
        {
            ulong result = _buff[_pos++];
            if (result < 128)
            {
                return result;
            }
            result &= 0x7f;
            int shift = 7;
            do
            {
                byte b = _buff[_pos++];
                result |= (ulong)(b & 0x7F) << shift;
                if (b < 0x80)
                {
                    return result;
                }
                shift += 7;
            }
            while (shift < 64);
            return result;
        }
        /*
        internal uint ReadRawLittleEndian32()
        {
            if (bufferPos + 4 <= bufferSize)
            {
                if (BitConverter.IsLittleEndian)
                {
                    var result = BitConverter.ToUInt32(buffer, bufferPos);
                    bufferPos += 4;
                    return result;
                }
                else
                {
                    uint b1 = buffer[bufferPos];
                    uint b2 = buffer[bufferPos + 1];
                    uint b3 = buffer[bufferPos + 2];
                    uint b4 = buffer[bufferPos + 3];
                    bufferPos += 4;
                    return b1 | (b2 << 8) | (b3 << 16) | (b4 << 24);
                }
            }
            else
            {
                uint b1 = ReadRawByte();
                uint b2 = ReadRawByte();
                uint b3 = ReadRawByte();
                uint b4 = ReadRawByte();
                return b1 | (b2 << 8) | (b3 << 16) | (b4 << 24);
            }
        }
        */
    }


}
