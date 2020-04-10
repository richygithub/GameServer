using SharedLib;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tutorial;
using UseLibuv;

namespace Area.PlayerHandler
{
    public static class HandleHelper
    {

        public static int WordCount(this Channel c )
        {
            return 1;
       }
    }
}

namespace UseLibuv{

    public class Client: ClientEnd
    {
        public Client(EventLoop loop) : base(loop)
        {

        }
        public void SendPacket()
        {

        }

    }

    static public class Area
    {
        static public class PlayerHandler
        {

            static public Task<int> GetUserNum(Channel c,string s) {

                TaskCompletionSource<int> ts = new TaskCompletionSource<int>();

                //c.Send();
                int len = OutputStream.GetSize(s);
                int pId = c.CurPacketId;

                int pSize = OutputStream.GetSize(pId);

                c.OStream.WriteInt32(len+pId );
                c.OStream.WriteString(s);

                c.Send();
                

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

    public class Proxy
    {


    }

    public class Helper
    {

        void Test()
        {

        }
        /*
        void @"a.b.c"()
        {
        }
        */

    }


    public static class HandleHelper 
    {
        //
        
        public static int WordCount(this Channel c)
        {
            return 1;
        }
        /*
        public static class Area 
        {
            public static class GateHandler
            {
                public static int GetNum(int count)
                {
                    //
                    return 0;
                }
            }

        }
        */

        static public async void Test()
        {
            //int i=HandleHelper.Area.GateHandler.GetNum(100);

            Channel c = new Channel();

            int i = await Area.PlayerHandler.GetUserNum(c,"abcd3");

        }


    }
}
