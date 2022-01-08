using System.Text;
using CodeAnalyzation.Generation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace CodeAnalyzation.Generators
{
    [Generator]
    public class DependencyGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            // Register a factory that can create our custom syntax receiver
        }
        public void Execute(GeneratorExecutionContext context)
        {
            var syntaxTrees = context.Compilation.SyntaxTrees;
            SyntaxNodeExtensions.SetCompilation(context.Compilation, syntaxTrees, "DependencyGenerator");
            var dependencies = DependencyGeneration.GenerateDependencies(syntaxTrees, context.Compilation);
            Logger.Print(context, "context.Compilation", $"{context.Compilation}");

            // inject the created source into the users compilation // inject the created source into the users compilation
            context.AddSource("dependencyGenerator.Generated.cs", SourceText.From(dependencies.NormalizeWhitespace().ToString(), Encoding.UTF8));

        }
    }
}