using System;
using System.Linq;
using LibraryB;
using Microsoft.CodeAnalysis;

namespace CodeAnalyzation.Generators
{
    // Cookbook: https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.cookbook.md
    [Generator]
    public class MySourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context) { }

        public void Execute(GeneratorExecutionContext context)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var libraryB = assemblies.FirstOrDefault(x => x.FullName.StartsWith("LibraryB"));
            var bTypes = libraryB?.GetExportedTypes();

            var assemblyNames = assemblies.Select(x => x.FullName);
            var bTypesStatements = bTypes.Select(x => x.FullName ?? "");

            // add the generated implementation to the compilation
            Logger.Print(context, "MySource", "Generated", "yaaay");
            Logger.Print(context, "B", new ClassB().Print(), "end B.");
            Logger.PrintFromCode(context, "A", "new A(): {new ClassA().Print()}");
            Logger.PrintFromCode(context, "assemblyNames", assemblyNames);
            Logger.PrintFromCode(context, "bTypesStatements", bTypesStatements);
        }
    }
}