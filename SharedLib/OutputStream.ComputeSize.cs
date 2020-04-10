using System;
using System.Collections.Generic;
using System.Text;

namespace UseLibuv 
{
    public partial class OutputStream
    {

        public static int GetSize(int value)
        {
            return ComputeRawVarint32Size((uint)value);
        }
        public static int GetSize(uint value)
        {
            return ComputeRawVarint32Size((uint)value);
        }
        public static int GetSize(String value)
        {
            return ComputeStringSize(value);
        }

        public static int GetSize(long value)
        {
            return ComputeRawVarint64Size((ulong)value);
        }
        public static int GetSize(ulong value)
        {
            return ComputeRawVarint64Size(value);
        }


         static int ComputeStringSize(String value)
        {
            int byteArraySize = Utf8Encoding.GetByteCount(value);
            return ComputeRawVarint32Size((uint)byteArraySize) + byteArraySize;
        }

         static int ComputeRawVarint32Size(uint value)
        {
            if ((value & (0xffffffff << 7)) == 0)
            {
                return 1;
            }
            if ((value & (0xffffffff << 14)) == 0)
            {
                return 2;
            }
            if ((value & (0xffffffff << 21)) == 0)
            {
                return 3;
            }
            if ((value & (0xffffffff << 28)) == 0)
            {
                return 4;
            }
            return 5;
        }

        /// <summary>
        /// Computes the number of bytes that would be needed to encode a varint.
        /// </summary>
         static int ComputeRawVarint64Size(ulong value)
        {
            if ((value & (0xffffffffffffffffL << 7)) == 0)
            {
                return 1;
            }
            if ((value & (0xffffffffffffffffL << 14)) == 0)
            {
                return 2;
            }
            if ((value & (0xffffffffffffffffL << 21)) == 0)
            {
                return 3;
            }
            if ((value & (0xffffffffffffffffL << 28)) == 0)
            {
                return 4;
            }
            if ((value & (0xffffffffffffffffL << 35)) == 0)
            {
                return 5;
            }
            if ((value & (0xffffffffffffffffL << 42)) == 0)
            {
                return 6;
            }
            if ((value & (0xffffffffffffffffL << 49)) == 0)
            {
                return 7;
            }
            if ((value & (0xffffffffffffffffL << 56)) == 0)
            {
                return 8;
            }
            if ((value & (0xffffffffffffffffL << 63)) == 0)
            {
                return 9;
            }
            return 10;
        }
    }
}
