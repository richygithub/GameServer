using System;
using System.Collections.Generic;
using System.Text;

namespace UseLibuv
{

    public class Attr
    {
        int hp;
        int att;
        int def;
    }

    public class Skill
    {
        int id;

    }

    public class Player
    {
        string name;
        Int64 guid;
        int id;
        Attr attr;
        Skill skill;
        
    }
    public class Monster
    {
        Int64 guid;
        int id;
        Attr attr;
        Skill skill;
 
    }

    public partial class AreaServer : GameServer<AreaServer>
    {

        const int MAX_PLAYER = 1024;
        const int MAX_MONSTER = 2048;

        Player[]    players = new  Player[MAX_PLAYER];
        Monster[]   monsters = new Monster[MAX_MONSTER];



        Handler<AreaServer> _handler = new AreaServerHandler();


        ServerEnd _serverEnd;// = new ServerEnd()

        EventLoop _eventLoop;
        public static readonly AreaServer instance = new AreaServer();
        private AreaServer()
        {
            _eventLoop = new EventLoop();
            _serverEnd = new ServerEnd(_eventLoop,11240);

        }
        public int GetUserNum()
        {
            return 1;
        }

        public override void Start()
        {
            //_serverEnd.Run();
            _serverEnd.Start();

            _eventLoop.Run();

        }
    }




}
