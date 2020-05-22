using Proto;
using SharedLib;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tutorial;
using UseLibuv;

namespace Client
{
    static public class Area
    {
        static public class PlayerHandler
        {

            static public Task<int> GetUserNum(string s)
            {

                TaskCompletionSource<int> ts = new TaskCompletionSource<int>();

                //c.Send();
                int len = OutputStream.GetSize(s);
                uint pId = NetClient.Instance.PacketId;
                var ostream= NetClient.Instance.OutputStream;

                int pSize = OutputStream.GetSize(pId);

                //ostream.WriteInt32(len );

                uint serviceId = Packet.GetService(1, 1);

                ostream.WritePacketHead((byte)PacketType.REQ, (uint)(pSize+len+OutputStream.GetSize(serviceId) ) );

                //ostream.WriteService(ser);
                ostream.WriteUInt32(serviceId);
                ostream.WriteUInt32(pId);
                ostream.WriteString(s);

                NetClient.Instance.AddCB(pId, (InputStream p) =>
                {
                    //
                    var ret = p.ReadInt32(); 
                    ts.SetResult(ret);
                });

                NetClient.Instance.Send();


                return ts.Task;
                //Area.PlayerHandler.
                //c.WordCount();
                //return 0; 
            }

            static public Task<int> GetTest(Channel c, AddressBook ab)
            {

                TaskCompletionSource<int> ts = new TaskCompletionSource<int>();

                //c.Send();


                return ts.Task;
                //Area.PlayerHandler.
                //c.WordCount();
                //return 0; 
            }

        }
        static public class SceneHandler
        {


        }

    }
}
