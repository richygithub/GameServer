using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLib
{
    public class PacketWrite
    {

        public byte[] Write(string str)
        {
            byte[] byteArray = System.Text.Encoding.Default.GetBytes(str);

            int len = byteArray.Length;
            byte[] msg = new byte[PacketRead.HeadLen + len];

            msg[0] = (byte)( len & 0xFF );
            msg[1] = (byte)( len>>8 & 0xFF );
            msg[2] = (byte)( len>>16 & 0xFF );
            msg[3] = (byte)( len>>24 & 0xFF );

            Buffer.BlockCopy(byteArray, 0, msg, PacketRead.HeadLen,len);

            return msg;

        }

    }
}
