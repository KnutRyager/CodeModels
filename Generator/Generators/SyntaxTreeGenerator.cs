using System;
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
            var classes = syntaxTrees.Classes();
            //var classes = classVisitor.GetEntries(syntaxTrees.Classes());

            //var firstClass = classes.First();
            //var semanticModel = context.Compilation.GetSemanticModel(firstClass.SyntaxTree, true);
            //var type = semanticModel.GetDeclaredSymbol(firstClass) as ITypeSymbol;
            //var models = classes.Where(x => x.AttributeLists.FirstOrDefault()?.GetType() == typeof(ModelAttribute));
            // add the filepath of each tree to the class we're building
            Logger.Print(context, "Title", "The following syntax trees existed in the compilation that created this program:");
            foreach (var model in classes)
            {
                Logger.Print(context, $"Syntax_{model}", model.ToString());
            }
        }
    }
}