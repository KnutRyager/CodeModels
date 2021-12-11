using System.Collections.Generic;
using System.Linq;
using CodeAnalyzation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Common.Extensions;
using Common.Files;

using static CodeAnalyzation.SyntaxNodeExtensions;

public static class Util
{
    public static SyntaxTree ParseTree(this string str)
        => CSharpSyntaxTree.ParseText(Mustache.RemoveMustache(str));

    public static CompilationUnitSyntax Parse(this string str)
    {
        var tree = ParseTree(str);
        SetSemanticModel(new[] { tree });
        return (CompilationUnitSyntax)tree.GetRoot();
    }

    public static IEnumerable<CompilationUnitSyntax> Parse(this IEnumerable<string> strs)
    {
        var trees = strs.Select(ParseTree).ToArray();
        SetSemanticModel(trees);
        return trees.Select(tree => (CompilationUnitSyntax)tree.GetRoot());
    }

    public static IEnumerable<CompilationUnitSyntax> ParsePath(this string path) => FileUtil.ReadFilesToText(FileUtil.GetFiles(path)).Parse();

    private static void SetSemanticModel(IEnumerable<SyntaxTree> trees)
    {
        var Mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        var compilationWithModel = CSharpCompilation.Create("MyCompilation",
            syntaxTrees: trees, references: new[] { Mscorlib });
        //Note that we must specify the tree for which we want the model.
        //Each tree has its own semantic model
        SetCompilation(compilationWithModel, trees);
    }

    public static CompilationUnitSyntax ParseFile(this string path)
        => FileUtil.ReadFileToText(path).Parse();

    public static string SingleQuote(this string str) => str.Replace("\"", "'");

    public static string ParseToJson(this string str)
    {
        var parsed = str.Parse();
        var @namespace = parsed.GetNamespaces().FirstOrDefault();
        var classes = parsed.GetClasses().Select(x => x.ParseToJson(@namespace));
        return JsonList(classes);
    }

    public static string ParseToJson(this ClassDeclarationSyntax c, NamespaceDeclarationSyntax? @namespace)
        => c.GetModel(@namespace).ToJson();

    public static string ParseFileToJson(this string path)
        => FileUtil.ReadFileToText(path).ParseToJson();

    public static string JsonList(IEnumerable<string> jsonObjects)
        => $"[{string.Join(",", jsonObjects)}]";
}