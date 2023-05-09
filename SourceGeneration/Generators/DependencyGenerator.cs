using System.Text;
using CodeModels.Extensions;
using CodeModels.Generation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace CodeModels.Generators;

[Generator]
public class DependencyGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context) { }

    public void Execute(GeneratorExecutionContext context)
    {
        var syntaxTrees = context.Compilation.SyntaxTrees;
        Extensions.SyntaxNodeExtensions.SetCompilation(context.Compilation, syntaxTrees, "DependencyGenerator");
        var dependencies = DependencyGeneration.GenerateDependencies(syntaxTrees, context.Compilation);
        var dependenciesCode = dependencies.NormalizeWhitespace().ToString();
        Logger.Print(context, nameof(DependencyGenerator), dependenciesCode);

        // inject the created source into the users compilation // inject the created source into the users compilation
        context.AddSource("dependencyGenerator.Generated.cs", SourceText.From(dependenciesCode, Encoding.UTF8));

    }
}
