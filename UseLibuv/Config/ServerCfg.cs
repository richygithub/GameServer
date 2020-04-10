using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UseLibuv
{
    public class ServerCfg
    {
        public int id;
        public string name;
        public string host;
        public int port;

        public bool frontEnd;
        public string frontHost;
        public int clientPort;

    }


    public class Singleton<T> where T:new()
    {
        static Singleton()
        {
        }
        private static readonly T _instance = new T();

        public static T Instance => _instance;


    }

    public class CfgMgr
    {
        const string serverCfgPath = "./servers.json";

        Dictionary<string, List<ServerCfg>> _serverCfgs = new Dictionary<string, List<ServerCfg>>();

        public List<ServerCfg> GetServerCfg(string name)
        {
            List<ServerCfg> cfg;
            if( _serverCfgs.TryGetValue(name,out cfg))
            {
                return cfg;
            }
            return null;
        }
        private CfgMgr()
        {
            //Console.WriteLine("private CfgMgr");

            using (StreamReader sr = new StreamReader(serverCfgPath))
            {
                var jobj = JObject.Parse(sr.ReadToEnd());
                foreach (var kv in jobj)
                {
                    Console.WriteLine($"key:{kv.Key}");

                    List<ServerCfg> slist = JsonConvert.DeserializeObject<List<ServerCfg>>(kv.Value.ToString());
                    _serverCfgs.Add(kv.Key, slist);
                   //Console.WriteLine($"key:{kv.Value}");
                }
            }
        }
        static CfgMgr()
        {
            //Console.WriteLine("static CfgMgr"); 
        }
        private static readonly CfgMgr _instance = new CfgMgr();
        public static CfgMgr Instance => _instance;




    }


 }
