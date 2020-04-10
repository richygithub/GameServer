using System;
using System.Collections.Generic;
using System.Text;

namespace UseLibuv
{

    abstract public class Handler<T>
    {

    }

    abstract public class Remote<T>
    {

    }

    abstract public class GameServer<T>
    {
        //public static readonly GameServer<T> instance = new GameServer<T>();

        //Handler<T> _handler = new Handler<T>();
        //Remote<T>  _remote  = new Remote<T>();
         abstract  public void Start();



    }







}
