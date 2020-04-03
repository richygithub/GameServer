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

        EventLoop _eventLoop;

        public Dispatcher(EventLoop loop)
        {
            _eventLoop = loop;
        }

        public void Process(EventLoop loop,Channel c, List<Packet> p)
        {
            //?

            if( _eventLoop != loop)
            {
                _eventLoop.AddAsyncJob(() =>
                {
                //process
                    Packet ret = new Packet();

                    loop.AddAsyncJob(() =>
                    {
                       //byte[] buff = loop.PWriter.Write("abcde");

                       c.Send("abcde",loop.PWriter);
                    });
                });
            }
            else
            {
                Packet ret = new Packet();
                //byte[] buff = loop.PWriter.Write("abcde");
                //c.Send(buff);
                c.Send("abcde",loop.PWriter);
 
            }
            

        }

/*
        public void Process(Packet p)
        {
            //p.head
            //route

            //解包--
            _maps.Add(1, () =>
            {
                

            });


        }
*/
    }
}
