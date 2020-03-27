using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.IO;

namespace CodeGenerator
{
    class Program
    {
        const string codegenDir = "Generated";

        const string serverDir= "Servers";

        const string handlerDirName = "Handler";

        const string remoteDirName = "Remote";



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


        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
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
