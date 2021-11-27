using System;
using System.IO;
using System.Linq;
using System.Text;
using LibraryB;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

[Generator]
public class AugmentingGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        // Register a factory that can create our custom syntax receiver
        context.RegisterForSyntaxNotifications(() => new MySyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        // the generator infrastructure will create a receiver and populate it
        // we can retrieve the populated instance via the context
        MySyntaxReceiver syntaxReceiver = (MySyntaxReceiver)context.SyntaxReceiver;

        // get the recorded user class
        ClassDeclarationSyntax userClass = syntaxReceiver.ClassToAugment;

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        var printStatements = string.Join("\n", assemblies.Select(x => x.FullName).Select(x => $"Console.WriteLine(\"{x}\");"));

        var libraryB = assemblies.FirstOrDefault(x => x.FullName.StartsWith("LibraryB"));

        var bTypes = libraryB?.GetExportedTypes();

        var bTypesStatements = string.Join("\n", bTypes.Select(x => $"Console.WriteLine(\"{x.FullName ?? ""}\");"));

        if (userClass is null)
        {
            // if we didn't find the user class, there is nothing to do
            return;
        }

        // add the generated implementation to the compilation
        SourceText sourceText = SourceText.From($@"
using LibraryA;
public partial class {userClass.Identifier}
{{
    private void GeneratedMethod()
    {{
        // generated code
        Write(""GENERATED"");
        Console.WriteLine(""EvenBetterSup"");
        Console.WriteLine(""hmmm"");
        Console.WriteLine($""hmmm4yay5 {{new ClassA().Print()}}"");
        Console.WriteLine($""hmmm4yay6 {new ClassB().Print()}"");
        Console.WriteLine(""BetterSup"");
        {printStatements}
        Write(""B TYPES:"");
        {bTypesStatements}
    }}
}}", Encoding.UTF8);
        context.AddSource("UserClass.Generated.cs", sourceText);
    }

    class MySyntaxReceiver : ISyntaxReceiver
    {
        public ClassDeclarationSyntax ClassToAugment { get; private set; } = default;

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            // Business logic to decide what we're interested in goes here
            if (syntaxNode is ClassDeclarationSyntax cds &&
                cds.Identifier.ValueText == "UserClass")
            {
                ClassToAugment = cds;
            }
        }
    }
}