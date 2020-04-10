using SharedLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace UseLibuv 
{
    public interface IDispatcher
    {
        void Process(EventLoop loop, Channel c, List<Packet> packets);
    }
}
