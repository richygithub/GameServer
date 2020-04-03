using System;
using System.Collections.Generic;
using System.Text;

namespace UseLibuv 
{
    public class ClientEnd
    {
        Dictionary<int, Channel> _servers = new Dictionary<int, Channel>();

        EventLoop _loop;


        public ClientEnd(EventLoop loop)
        {
            _loop = loop;

        }


        public void Connect(string host, int port)
        {

        }

    }

}
