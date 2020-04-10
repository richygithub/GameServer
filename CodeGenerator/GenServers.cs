using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace CodeGenerator
{
    public static class GenServers
    {
        public static string content = @"
public class Program{
    public void HelloWorld(){
        Console.Writeline(""Hello World!"");
    }
}
";
        public static void GenerateServers(JObject jobj, string projdir)
        {
            CSharpSyntaxTree cst = CSharpSyntaxTree.ParseText(content, CSharpParseOptions.Default, "HelloWorld.cs") as CSharpSyntaxTree;


            var root = cst.GetRoot();
            //root.
            IEnumerable<MethodDeclarationSyntax> methodsDeclarations = root.DescendantNodes().OfType<MethodDeclarationSyntax>();

            var metaRefSystem = MetadataReference.CreateFromFile(typeof(String).Assembly.Location);
            var metaRefs = ImmutableArray.Create<MetadataReference>(metaRefSystem);

            //CSharpCompilationOptions compilationOptions = new CSharpCompilationOptions();
            //var compilation = CSharpCompilation.Create("GenerateScriptAssembly", , metaRefs);


        }
    }
}
