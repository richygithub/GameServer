using SharedLib;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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



        Handler<AreaServer> _handler = new PlayerHandler();


        ServerEnd _serverEnd;// = new ServerEnd()

        EventLoop _netLoop,_mainLoop;

        public static readonly AreaServer instance = new AreaServer();
        private AreaServer()
        {
            _netLoop = new EventLoop();
            _mainLoop = new EventLoop();

             

            _serverEnd = new ServerEnd(_netLoop,new Dispatcher(_mainLoop), 11240);

        }

        public int GetUserNum()
        {
            return 1;
        }

        public override void Start()
        {
            //_serverEnd.Run();
            _serverEnd.Start();

            Task netTask = Task.Factory.StartNew(() =>
            {
                _netLoop.Run();
            }, CancellationToken.None, TaskCreationOptions.LongRunning,TaskScheduler.Default);

            netTask.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted) ;


            _mainLoop.Run(100);

        }
        static void ExceptionHandler(Task task)
        {
            var exception = task.Exception;
            Console.WriteLine(exception);
        }

    }




}
