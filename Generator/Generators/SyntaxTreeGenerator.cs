using System;
using System.Linq;
using System.Text;
using CodeAnalyzation.Collectors;
using Common.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Models;

namespace CodeAnalyzation.Generators
{
    [Generator]
    public class SyntaxTreeGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            // Register a factory that can create our custom syntax receiver
        }
        public void Execute(GeneratorExecutionContext context)
        {
            // begin creating the source we'll inject into the users compilation
            StringBuilder sourceBuilder = new(@"
using System;
namespace HelloWorldGenerated
{
    public static class HelloWorld
    {
        public static void SayHello() 
        {
            Console.WriteLine(""Hello from generated code!"");
            Console.WriteLine(""The following syntax trees existed in the compilation that created this program:"");
");

            // using the context, get a list of syntax trees in the users compilation
            var syntaxTrees = context.Compilation.SyntaxTrees;

            var classVisitor = new ClassVisiter(new ClassFilter(attributes: new Type[] { typeof(ModelAttribute) }));
            var classes = classVisitor.GetEntries(syntaxTrees);

            //var firstClass = classes.First();
            //var semanticModel = context.Compilation.GetSemanticModel(firstClass.SyntaxTree, true);
            //var type = semanticModel.GetDeclaredSymbol(firstClass) as ITypeSymbol;
            //var models = classes.Where(x => x.AttributeLists.FirstOrDefault()?.GetType() == typeof(ModelAttribute));
            // add the filepath of each tree to the class we're building
            foreach (var model in classes)
            {
                sourceBuilder.AppendLine($@"Console.WriteLine(@""{StringUtil.CleanCodeString(model.ToString())}"");");
            }

            // finish creating the source to inject
            sourceBuilder.Append(@"
        }
    }
}");

            // inject the created source into the users compilation // inject the created source into the users compilation
            context.AddSource("helloWorldGenerator", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));

        }
    }
}