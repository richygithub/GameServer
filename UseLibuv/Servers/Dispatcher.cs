using SharedLib;
using System;
using System.Collections.Generic;
using System.Text;
using UseLibuv;
using Proto;

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
            _remoteServers = new RemoteServers(_eventLoop);
        }
        public void Forward(Channel c,Packet p)
        {
            byte packetType = p.packetType;

            //uint service = byte serverType = Packet.GetServiceServerType(service);

            //uint serviceId = Packet.GetServiceId(service);
            byte serverType = Packet.GetServiceServerType(p.serviceId);

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




    public class Dispatcher :IDispatcher
    {
        Dictionary<int, Action> _maps = new Dictionary<int, Action>();
        Dictionary<int, PacketProcess> _funcs = new Dictionary<int, PacketProcess>();

        delegate void PacketProcess(Channel c, Packet p);

        EventLoop _workLoop;

        Dictionary<int, WeakReference<Channel>> _cb = new Dictionary<int, WeakReference<Channel>>();

        public Dispatcher(EventLoop loop)
        {
            _workLoop = loop;
            _remoteServer = new RemoteServers(loop);
        }
        InputStream istream = new InputStream();

        RemoteServers _remoteServer;//= new RemoteServers();

        byte _serverType = 0;

        int _transferId = 0;

        void Register()
        {
            /*
            = () =>
            {
                
            }
            */
        }

        void HandlePacket(EventLoop netloop,Channel c, Packet p)
        {

            if (_workLoop != netloop)
            {
                _workLoop.AddAsyncJob(() =>
                {
                    //byte[] buff = loop.PWriter.Write("abcde");

                    //c.Send("abcde",loop.PWriter);

                    int xx;
                    string bbb;
                    switch (p.serviceId){
                        case 1:
                            break;
                        default:
                            break;
                    }

                    netloop.AddAsyncJob(() =>
                    {

                        c.Send();
                    });
                    
                    //c.Send()



                });
            }
            else
            {
                Packet ret = new Packet();
                //byte[] buff = loop.PWriter.Write("abcde");
                //c.Send(buff);
                //c.Send("abcde",loop.PWriter);

            }
        } 
        void ForwardPacket(EventLoop netloop, Packet p)
        {
            //直接send

        }
        
        public void Process(EventLoop netloop,Channel c, List<Packet> packets)
        {
            //?
            foreach(var p in packets)
            {
                istream.SetUp(p.body, p.len);


                //byte serverType = istream.ReadByte();
                byte packetType = p.packetType;
                //p.serverType = serverType;
                p.packetType = packetType;
                uint service = istream.ReadUInt32();
                p.serviceId = service;

                byte serverType = Packet.GetServiceServerType(service);

                uint serviceId  = Packet.GetServiceId(service);


                Console.WriteLine($"Process Server Type:{serverType},{serviceId}");

                if(_serverType == serverType)
                {
                    //process
                    HandlePacket(netloop,c, p);

                }
                else
                {
                    //router
                    ForwardPacket(netloop, p);

                    //_remoteServer[serverType]?.Forward(p);

                }




            }


            if ( _workLoop != netloop)
            {
                _workLoop.AddAsyncJob(() =>
                {
                //process
                    Packet ret = new Packet();

                    netloop.AddAsyncJob(() =>
                    {
                       //byte[] buff = loop.PWriter.Write("abcde");

                       //c.Send("abcde",loop.PWriter);
                    });
                });
            }
            else
            {
                Packet ret = new Packet();
                //byte[] buff = loop.PWriter.Write("abcde");
                //c.Send(buff);
                //c.Send("abcde",loop.PWriter);
 
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
