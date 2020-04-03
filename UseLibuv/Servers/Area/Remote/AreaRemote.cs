using System;
using System.Collections.Generic;
using System.Text;

namespace UseLibuv
{
    public class AreaServerRemote : Remote<AreaServer>
    {
        public int GetUserNum()
        {
            return AreaServer.instance.GetUserNum();
        }
    }
}
