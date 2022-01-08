using System;
using System.Linq;
using CodeAnalyzation.Collectors;
using CodeAnalyzation.Generation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Models;

namespace CodeAnalyzation.Generators
{
    //[Generator]
    public class SyntaxTreeGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context) { }

        public void Execute(GeneratorExecutionContext context)
        {
            // using the context, get a list of syntax trees in the users compilation
            var syntaxTrees = context.Compilation.SyntaxTrees;

            var classVisitor = new ClassVisiter(new(attributes: new Type[] { typeof(ModelAttribute) }));
            var classes = classVisitor.GetEntries(syntaxTrees);

            Logger.Print(context, "Title", $"The following {syntaxTrees.Count()} syntax trees existed in the compilation that created this program:");
            Logger.Print(context, $"Syntax_trees", syntaxTrees.Select(x => x.GetRoot().ToString()).ToArray());
            Logger.Print(context, $"{classes.Count()} classes", classes.Select(x => x.ToString()).ToArray());
        }
    }
}