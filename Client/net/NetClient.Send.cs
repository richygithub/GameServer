using Proto;
using SharedLib;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UseLibuv;

namespace Client
{
    public partial class NetClient
    {
        uint _packetId = 1;
        public uint PacketId => _packetId;

        OutputStream _ostream;
        InputStream _istream = new InputStream();

        public OutputStream OutputStream => _ostream;
        Dictionary<uint, Action<InputStream> > _dic = new Dictionary<uint, Action<InputStream>>();

        public void AddCB(uint id,Action<InputStream> cb )
        {
            _dic.Add(id, cb);

        }

        public void Process(Packet p)
        {
            uint pid = p.seqId;
            Action<InputStream> cb;
            if( _dic.TryGetValue(pid,out cb))
            {
                _istream.SetUp(p.body,p.len);
                cb(_istream);
            }

        }

        public void Send()
        {
            if(_socket.Connected)
            {
                _socket.Send(_ostream.Buff, 0, _ostream.Length,SocketFlags.None);
            }
        }


    }
}
