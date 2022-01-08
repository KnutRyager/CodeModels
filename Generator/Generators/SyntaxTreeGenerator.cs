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
    [Generator]
    public class SyntaxTreeGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context) { }

        public void Execute(GeneratorExecutionContext context)
        {
            // using the context, get a list of syntax trees in the users compilation
            var syntaxTrees = context.Compilation.SyntaxTrees;

            var classVisitor = new ClassVisiter2();
            //var classVisitor = new ClassVisiter(new(attributes: new Type[] { typeof(ModelAttribute) }));
            var allNodes = syntaxTrees.SelectMany(x => x.GetRoot().DescendantNodes()).ToList();
            var e = classVisitor.GetEntries(syntaxTrees.First().GetRoot().DescendantNodes());
            //var d = syntaxTrees.First().GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>() ;
            var classesOnly = allNodes.Where(x => x is ClassDeclarationSyntax).ToList();
            var classes = syntaxTrees.SelectMany(x => x.GetRoot().GetClasses()).ToList();
            //var classes = classVisitor.GetEntries(syntaxTrees.Classes());

            //var firstClass = classes.First();
            //var semanticModel = context.Compilation.GetSemanticModel(firstClass.SyntaxTree, true);
            //var type = semanticModel.GetDeclaredSymbol(firstClass) as ITypeSymbol;
            //var models = classes.Where(x => x.AttributeLists.FirstOrDefault()?.GetType() == typeof(ModelAttribute));
            Logger.Print(context, "Title", $"The following {syntaxTrees.Count()} syntax trees existed in the compilation that created this program:");
            //Logger.Print(context, $"Syntax_trees", syntaxTrees.Select(x => x.GetRoot().ToString()).ToArray());
            //Logger.Print(context, $"{classesOnly.Count()} classes", allNodes.Select(x => x.ToString()).ToArray());
            Logger.Print(context, $"{e.Count()} e", e.Select(x=>x.ToString()).ToArray());
        }
    }
}