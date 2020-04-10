using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace CodeGenerator
{
    class Program
    {
        const string codegenDir = "Generated";

        const string serverDir= "Servers";

        const string handlerDirName = "Handler";

        const string remoteDirName = "Remote";

        const string configDirName = "Config";




        static void ProcessHandler(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            foreach(var file in dir.GetFiles())
            {
                Console.WriteLine(file.FullName);
                string text = File.ReadAllText(file.FullName);

                SyntaxTree tree = CSharpSyntaxTree.ParseText(text);
                CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
                Console.WriteLine($"The tree is a {root.Kind()} node.");
                Console.WriteLine($"The tree has {root.Members.Count} elements in it.");
                foreach( var m in root.Members)
                {
                    var nm = (NamespaceDeclarationSyntax)m;

                    Console.WriteLine($"mkind:{m.Kind()},{nm.Name}:{nm.Members.Count}");
                    foreach(var inm in nm.Members)
                    {
                        var cnm = (ClassDeclarationSyntax)inm;
                        //Console.WriteLine($"inmkind:{inm.Kind()}");
                        foreach(var cmm in cnm.Members)
                        {
                            if(cmm.Kind()==SyntaxKind.MethodDeclaration)
                            {
                                var method = (MethodDeclarationSyntax)cmm;
                                Console.WriteLine($"method:{method.Identifier}," +
                                    $"parm:{method.ParameterList},body:{method.Body},modify:{method.Modifiers}");

                            }
                            else if(cmm.Kind()==SyntaxKind.FieldDeclaration)
                            {
                                //root.RemoveNode(cmm);
                                //cnm.RemoveNode<FieldDeclarationSyntax>((FieldDeclarationSyntax)cmm);
                            }

                            Console.WriteLine($"cmem:{cmm.Kind()}");
                        }
 
                    }
                }
                Console.WriteLine($"The tree has {root.Usings.Count} using statements. They are:");
                Console.WriteLine($"fullstr:{root.ToFullString()}");
                /*
                foreach (UsingDirectiveSyntax element in root.Usings)
                    Console.WriteLine($"\t{element.Name}");
                    */

            }


        }
        static void ProcessRemote(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            foreach (var file in dir.GetFiles())
            {
                Console.WriteLine(file.FullName);

            }
        }

        class ServerCfg
        {
            public int id;

            [JsonProperty]
            public string name { get; }
            public string frontHost;
            public bool frontEnd;
        }


        public class CfgMgr
        {
            private CfgMgr()
            {

                Console.WriteLine("private CfgMgr");
            }
            static CfgMgr()
            {
                Console.WriteLine("static CfgMgr");
            }
            private static readonly CfgMgr _instance = new CfgMgr();
            public static CfgMgr Instance => _instance;




        }

        static void ProcessConfig(string dir)
        {
            string serverCfg = Path.Join(dir, configDirName, "servers.json");
            //var c = CfgMgr.Instance;
            if (!File.Exists(serverCfg))
            {
                Console.WriteLine($"Error! not find config file!{serverCfg}");
                return;
            }
            //JsonConvert.
            string json = "[{'id':1,'age':100,'name':'abcde'},{'id':2,'name':'abcde1'}]";
            List<ServerCfg> a = JsonConvert.DeserializeObject< List<ServerCfg> >(json);
            Console.WriteLine($"{a[1].id}:{a[1].name}");
            using ( StreamReader sr = new StreamReader(serverCfg) )
            {
                var jobj =JObject.Parse( sr.ReadToEnd());
                foreach(var kv in jobj)
                {
                    Console.WriteLine($"key:{kv.Key}");
 
                    List<ServerCfg> slist = JsonConvert.DeserializeObject<List<ServerCfg>>(kv.Value.ToString());
                    foreach(var s in slist)
                    {
                        Console.WriteLine($"{s.id},{s.name},{s.frontHost==null},{s.frontEnd}");
                    }
                    //Console.WriteLine($"key:{kv.Value}");
                }

                

                GenServers.GenerateServers(jobj, dir);
            }



        }
        static int __LINE__([System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            return lineNumber;
        }
        static string __FILE__([System.Runtime.CompilerServices.CallerFilePath] string fileName = "")
        {
            return fileName;
        }
        static void Main(string[] args)
        {
           string projDir;
            if ( args.Length>0)
            {
                projDir = args[0];
            }
            else
            {
                projDir = "F:\\C#Proj\\UseLibuv\\UseLibuv";

            }
            string dir = Path.Join(projDir, codegenDir);
            if(!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            dir = Path.Join(projDir, serverDir);
            if (!Directory.Exists(dir))
            {
                //编译错误
                return;
            }

            ProcessConfig(projDir);
            return;

            DirectoryInfo serverRoot = new DirectoryInfo(dir);

            foreach(var d in serverRoot.GetDirectories() )
            {
                var serverHandlerDir = Path.Join(d.FullName, handlerDirName);
                if (Directory.Exists(serverHandlerDir))
                {
                    ProcessHandler(serverHandlerDir);
                }
                var remoteHandlerDir = Path.Join(d.FullName,remoteDirName);
                if (Directory.Exists(remoteHandlerDir))
                {
                    ProcessRemote(remoteHandlerDir);
                }

            }








            foreach(var s in args)
            {
                Console.WriteLine(s);
            }

        }
    }
}
