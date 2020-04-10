using SharedLib;
using System;
using System.Collections.Generic;
using System.Text;
using UseLibuv;

namespace UseLibuv 
{

    public class RpcServer
    {

    }

    public class RpcClient
    {

        uint _seqId = 0;
        Dictionary<uint, WeakReference<Channel>> _cb = new Dictionary<uint, WeakReference<Channel>>();
        RemoteServers _remoteServers;

        const int _respTimeout = 1000*5;
        EventLoop _eventLoop;
        public RpcClient(EventLoop loop)
        {
            _eventLoop = loop;
            _remoteServers = new RemoteServers();
        }
        public void Forward(Channel c,Packet p)
        {
            byte packetType = p.packetType;
            byte serverType = p.serverType;

            if ((byte)PacketType.REQ == packetType)
            {
                if (_seqId == 0)
                {
                    ++_seqId;
                }

                _cb.Add(_seqId, new WeakReference<Channel>(c));
                //_eventLoop.
                //_transferId
                uint id = _seqId;
                p.seqId = _seqId;
                _eventLoop.StartTimer(_respTimeout, false, () =>
                {
                    WeakReference<Channel> weakobj;
                    if (_cb.TryGetValue(id, out weakobj))
                    {
                        Channel targetChannel;
                        if (weakobj.TryGetTarget(out targetChannel))
                        {
                            //send Error
                        }
                        _cb.Remove(id);
                    }
                });

                _remoteServers[serverType]?.Forward(p);

                _seqId++;
                //
            }
            else if ((byte)PacketType.NOTIFY == packetType)
            {
                _remoteServers[serverType]?.Forward(p);

            }

        }

    }

    public enum PacketType
    {
        HEART_BEAT = 0,
        REQ        = 1,
        NOTIFY     = 2,
        RESP       = 3,
        PUSH       = 4, 
    }


    public class Dispatcher :IDispatcher
    {
        Dictionary<int, Action> _maps = new Dictionary<int, Action>();
        Dictionary<int, PacketProcess> _funcs = new Dictionary<int, PacketProcess>();

        delegate void PacketProcess(Channel c, Packet p);

        EventLoop _eventLoop;

        Dictionary<int, WeakReference<Channel>> _cb = new Dictionary<int, WeakReference<Channel>>();

        public Dispatcher(EventLoop loop)
        {
            _eventLoop = loop;
            _remoteServer = new RemoteServers(loop);
        }
        InputStream istream = new InputStream();

        RemoteServers _remoteServer;//= new RemoteServers();

        byte _serverType = 0;

        int _transferId = 0;


        public void Process(EventLoop loop,Channel c, List<Packet> packets)
        {
            //?
            foreach(var p in packets)
            {
                istream.SetUp(p.body, p.len);

                byte serverType = istream.ReadByte();

                byte packetType = istream.ReadByte();
                p.serverType = serverType;
                p.packetType = packetType;

                if(_serverType == serverType)
                {
                    //process

                }
                else
                {
                    //router


                    _remoteServer[serverType]?.Forward(p);

                }




                int serviceId = istream.ReadInt32();
                //check router or process





            }


            if ( _eventLoop != loop)
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
