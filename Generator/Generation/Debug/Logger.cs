using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeAnalyzation.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using static CodeAnalyzation.Models.CodeModelFactory;

public static class Logger
{
    public static void Print(GeneratorExecutionContext context, string tag, params string[] logs)
    {
        var model = StaticClass("GeneratorLog",
            PropertyCollection(Field($"log_{tag}", Value(logs))),
            topLevelModifier: Modifier.Partial, memberModifier: Modifier.Public);
        context.AddSource($"{tag}_log.Generated.cs", SourceText.From(model.Code(), Encoding.UTF8));
    }

    public static void PrintFromCode(GeneratorExecutionContext context, string tag, IEnumerable<string> logs)
         => PrintFromCode(context, tag, new string[] { }, logs);
    public static void PrintFromCode(GeneratorExecutionContext context, string tag, string[] usings, IEnumerable<string> logs)
    {
        var usingStatements = string.Join("\n", usings.Select(x => $"using {x};"));
        var printStatements = string.Join("\n", logs.Select(x => $"Console.WriteLine($\"{x}\");"));

        var sourceText = $@"
{usingStatements}
public static partial class GeneratorLog
{{
    public static void Print_{tag}()
    {{
        // generated code
        Console.WriteLine(""{tag}:"");
        {printStatements}
    }}
}}";

        context.AddSource($"print_{tag}.Generated.cs", SourceText.From(sourceText, Encoding.UTF8));
    }

    public static void PrintFromCode(GeneratorExecutionContext context, string tag, string[] usings, params string[] logs)
        => PrintFromCode(context, tag, usings, logs.ToList());
}