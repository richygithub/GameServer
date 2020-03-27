using System;
using System.Collections.Generic;
using System.Text;
using UseLibuv;

namespace SharedLib
{



    public class Dispatcher
    {
        Dictionary<int, Action> _maps = new Dictionary<int, Action>();
        Dictionary<int, PacketProcess> _funcs = new Dictionary<int, PacketProcess>();

        delegate void PacketProcess(Channel c, Packet p);
        public void Process(Packet p)
        {
            //p.head
            //route

            //解包--
            _maps.Add(1, () =>
            {
                

            });


        }
    }
}
