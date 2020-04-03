using System;
using System.Collections.Generic;
using System.Text;

namespace UseLibuv
{
    public class PlayerHandler : Handler<AreaServer>
    {

        public int id = 0;
        public const string x = "abcd";

        static void test()
        {

        }
        int GetNum()
        {
            return id;
        }

        //注释
        public int GetUserNum(string str)
        {
            return AreaServer.instance.GetUserNum();

        }

    }

}
