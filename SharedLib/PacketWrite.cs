using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLib
{
    public class PacketWrite
    {

        public byte[] Write(string str,int packetId)
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

        int WriteInt(int value,byte[] buff,int offset)
        {
            buff[offset + 0] = (byte)(value & 0xFF);
            buff[offset + 1] = (byte)(value >> 8 & 0xFF);
            buff[offset + 2] = (byte)(value >> 16 & 0xFF);
            buff[offset + 3] = (byte)(value >> 24 & 0xFF);
            return 4;
        }
        public int Write(int packetId,string str,byte[] buff,int offset)
        {
            //to do -- 判断长度。
            byte[] byteArray = System.Text.Encoding.Default.GetBytes(str);

            int len = byteArray.Length;
            //byte[] msg = new byte[PacketRead.HeadLen + len];

            offset += WriteInt(len,buff,offset);
            offset += WriteInt(packetId,buff,offset);



            Buffer.BlockCopy(byteArray, 0, buff, offset, len);

            return PacketRead.HeadLen + len;
        }



    }
}
