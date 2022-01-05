using System.Text;
using CodeAnalyzation.Generation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace CodeAnalyzation.Generators
{
    //[Generator]
    public class DependencyGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            // Register a factory that can create our custom syntax receiver
        }
        public void Execute(GeneratorExecutionContext context)
        {
            var syntaxTrees = context.Compilation.SyntaxTrees;

            var dependencies = DependencyGeneration.GenerateDependencies(syntaxTrees, context.Compilation);

            // inject the created source into the users compilation // inject the created source into the users compilation
            context.AddSource("dependencyGenerator", SourceText.From(dependencies.ToString(), Encoding.UTF8));

        }
    }
}