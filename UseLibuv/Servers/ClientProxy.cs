using Proto;
using SharedLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace UseLibuv
{

    public class ClientProxy
    {

        ClientEnd[] _clientRpc;
        delegate int Router(ClientEnd[] servers);

        Router route;

        EventLoop _eventLoop;
        public ClientProxy(EventLoop loop,List<ServerCfg> servers)
        {
            _eventLoop = loop;
            _clientRpc = new ClientEnd[ servers.Count ];

            for (int idx = 0; idx < servers.Count; idx++)
            {
                var cfg = servers[idx];
                _clientRpc[idx] = new ClientEnd(_eventLoop, cfg.host,cfg.port);
            }
        }
        public void Forward(Packet p )
        {
            int idx = route == null ? route(_clientRpc) : 0;
            //_servers[idx].OutputStream.(p);

        }


    }
    public class RemoteServerEnd
    {
        int id;
        internal Channel c;
        public RemoteServerEnd(ServerCfg cfg)
        {


        }



    }

    #region Generated
    public class RemoteServers
    {
        public const int AREA = 0;
        public const int CONNECTOR = 1;
        public const int CHAT = 2;
        public const int GATE = 3;

        public const int SERVER_NUM = 4;


        ClientProxy[] _servers = new ClientProxy[SERVER_NUM];

        public ClientProxy this[int i]
        {
            get
            {
                return i >= 0 && i < SERVER_NUM ? _servers[i] : null;
            }
        }
        Dictionary<string, int> _maps = new Dictionary<string, int>();
        public RemoteServers(EventLoop eventLoop)
        {
            _maps.Add("area", AREA);
            _maps.Add("connector", CONNECTOR);
            _maps.Add("chat", CHAT);
            _maps.Add("gate", GATE);

            foreach(var kv in _maps)
            {
                var cfg = CfgMgr.Instance.GetServerCfg(kv.Key);
                _servers[kv.Value] = new ClientProxy(eventLoop, cfg);
            }


            /*
            for(int idx = 0; idx < SERVER_NUM; idx++)
            {
                _servers[idx] = new ClientProxy( )
            }
            */

        }


    }
    #endregion

}
