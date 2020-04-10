using NUnit.Framework;
using System;
using UseLibuv;
using SharedLib;
using System.Collections.Generic;

namespace NUnitTest
{


    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            ostream = new OutputStream(bytes);
            istream = new InputStream(bytes);
        }

        byte[] bytes = new byte[1024];
        OutputStream ostream;// new OutputStream(new byte[1024]);
        InputStream istream;



        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(128)]
        [TestCase(1 << 14)]
        [TestCase(1 << 21)]
        [TestCase(1 << 28)]
        [TestCase(-(1 << 14))]
        [TestCase(-(1 << 21))]
        [TestCase(-(1 << 28))]

        public void Test_varint(int value)
        {
            ostream.WriteInt32(value);
            int ret = istream.ReadInt32();
            Assert.IsTrue(ret == value);


        }
        [TestCase(-1, 0, 1, 128, 1 << 14, 1 << 21, 1 << 28)]
        public void Test_varint(params int[] values)
        {
            int len = 0;
            foreach (var v in values)
            {
                ostream.WriteInt32(v);
                len += OutputStream.GetSize(v);
            }
            Assert.IsTrue(len == ostream.Length && ostream.Length > 0);
            for (int idx = 0; idx < values.Length; idx++)
            {
                int ret = istream.ReadInt32();
                Assert.IsTrue(ret == values[idx]);
            }


        }

        [TestCase("abcde")]
        public void Test_string(string value)
        {
            ostream.WriteString(value);
            string ret = istream.ReadString();
            Assert.IsTrue(ret == value, $"ret:{ret}");
        }

        [TestCase(-1, 0, 1, 128, 1 << 14, 1 << 21, 1 << 28, 1 << 35, 1 << 42 + 1, 1 << 49 + 1, 1 << 56 + 1, 1 << 63 + 1,
            -(1 << 14), -(1 << 21), -(1 << 28), -(1 << 35), -(1 << 42) - 1, -(1 << 49) - 1, -(1 << 56) - 1, -(1 << 62) - 1
            )]
        public void Test_int64(params long[] values)
        {
            int len = 0;
            foreach (var v in values)
            {
                ostream.WriteInt64(v);
                len += OutputStream.GetSize(v);
            }

            Assert.IsTrue(len == ostream.Length);
            for (int idx = 0; idx < values.Length; idx++)
            {
                long ret = istream.ReadInt64();
                Assert.IsTrue(ret == values[idx]);
            }
        }


        int GetSize(Object obj)
        {
            Type t = obj.GetType();
            if (t == typeof(int))
            {
                return OutputStream.GetSize((int)obj);
            }
            else if (t == typeof(long))
            {
                return OutputStream.GetSize((long)obj);

            }
            else if (t == typeof(string))
            {
                return OutputStream.GetSize((string)obj);
            }
            else
            {
                Console.WriteLine($"Error!Can not process type:{t}");
                return 0;
            }

        }
        void WriteToStream(OutputStream s, object obj)
        {
            Type t = obj.GetType();
            if (t == typeof(int))
            {
                s.WriteInt32((int)obj);
            }
            else if (t == typeof(long))
            {
                s.WriteInt64((long)obj);
            }
            else if (t == typeof(string))
            {
                s.WriteString((string)obj);
            }
            else
            {
                Console.WriteLine($"Error!Can not process type:{t}");
            }

        }


        [TestCase(10, "abcde", (long)1 << 62)]
        public void Test_packet(params object[] values)
        {
            int id = 3;

            int size = 0;
            foreach (var v in values)
            {
                size += GetSize(v);
            }
            int psize = OutputStream.GetSize(id);

            ostream.WriteInt32(psize + size);
            ostream.WriteInt32(id);

            foreach (var v in values)
            {
                WriteToStream(ostream, v);
            }

            Console.WriteLine($"PacketLen:{ostream.Length}");

            PacketRead pr = new PacketRead();
            List<Packet> packets = new List<Packet>();

            int smallBuffLen = 3;
            byte[] smallBuff = new byte[smallBuffLen];

            for(int i = 0; i < ostream.Length; )
            {
                int count = smallBuffLen;
                int left = ostream.Length - i;
                if( left < count)
                {
                    count = left;
                }

                Buffer.BlockCopy(bytes, i, smallBuff,0,count);

                pr.process(smallBuff,0, count, packets);
                i += smallBuffLen;
            }

            Assert.IsTrue(packets.Count == 1);

            Packet p = packets[0];
            InputStream stream = new InputStream(p.body);

            int id2 = stream.ReadInt32();
            Assert.IsTrue(id == id2);
            foreach (var obj in values)
            {
                //WriteToStream(ostream, v);
                Type t = obj.GetType();
                if (t == typeof(int))
                {
                    Assert.IsTrue((int)obj == stream.ReadInt32());
                }
                else if (t == typeof(long))
                {
                    Assert.IsTrue((long)obj == stream.ReadInt64());
                }
                else if (t == typeof(string))
                {
                    string s = stream.ReadString();
                    Assert.IsTrue((string)obj == s,$"{obj},{s}");
                    //Assert.AreEqual((string)obj, s);
                }

            }






        }

    }
}