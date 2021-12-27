using System.Collections.Generic;
using System.Linq;
using CodeAnalyzation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Common.Extensions;
using Common.Files;

using static CodeAnalyzation.SyntaxNodeExtensions;
using CodeAnalyzation.Models;
using System;

public static class Util
{
    public static SyntaxTree SyntaxTree(this string str, string? key = null, SourceCodeKind kind = SourceCodeKind.Regular) => str.Parse(key, kind).SyntaxTree;
    public static ExpressionSyntax ExpressionTree(this string str, string? key = null) => SyntaxFactory.ParseExpression(str);

    public static SyntaxTree[] SyntaxTrees(this string str, string? key = null) => new[] { str.Parse(key).SyntaxTree };

    public static CSharpSyntaxTree ParseTree(this string str, SourceCodeKind kind = SourceCodeKind.Regular)
        => (CSharpSyntaxTree.ParseText(Mustache.RemoveMustache(str), options: new CSharpParseOptions(kind: kind)) as CSharpSyntaxTree)!;

    public static CompilationUnitSyntax Parse(this string str, string? key = null, SourceCodeKind kind = SourceCodeKind.Regular)
    {
        var tree = ParseTree(str, kind);
        return StoreSyntaxTree(tree, key ?? str).GetCompilationUnitRoot();
    }

    public static (CompilationUnitSyntax Compilation, SemanticModel Model) ParseAndKeepSemanticModel(this string str, string? key = null, SourceCodeKind kind = SourceCodeKind.Regular)
        => (Compilation: str.Parse(key, kind), GetSemanticModel(key: str));

    public static IEnumerable<CompilationUnitSyntax> Parse(this IEnumerable<string> strs, string? key = null)
    {
        var trees = strs.Select(x => ParseTree(x)).ToArray();
        SetSemanticModel(trees, key ?? string.Join(", ", strs));
        return trees.Select(tree => (CompilationUnitSyntax)tree.GetRoot());
    }

    public static IEnumerable<CompilationUnitSyntax> ParsePath(this string path) => FileUtil.ReadFilesToText(FileUtil.GetFiles(path)).Parse();

    private static void SetSemanticModel(IEnumerable<SyntaxTree> trees, string? key = null)
    {
        var Mscorlib = GetReference<object>();
        var linqLib = GetReference(typeof(Enumerable));
        var collectionsLib = GetReference(typeof(List<>));
        var compilationWithModel = CSharpCompilation.Create("MyCompilation",
            syntaxTrees: trees, references: new[] { Mscorlib, linqLib, collectionsLib });
        //Note that we must specify the tree for which we want the model.
        //Each tree has its own semantic model
        SetCompilation(compilationWithModel, trees, key);
    }

    private static PortableExecutableReference GetReference<T>() => GetReference(typeof(T));
    private static PortableExecutableReference GetReference(Type type) => MetadataReference.CreateFromFile(type.Assembly.Location);

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

    public static string ParseToJson(this ClassDeclarationSyntax c, NamespaceDeclarationSyntax? @namespace) => ClassModel.Parse(c, @namespace).ToJson();
    public static string ParseFileToJson(this string path) => FileUtil.ReadFileToText(path).ParseToJson();
    public static string JsonList(IEnumerable<string> jsonObjects) => $"[{string.Join(",", jsonObjects)}]";

    private static T StoreSyntax<T>(T node, string? key = null) where T : CSharpSyntaxNode
    {
        StoreSyntaxTree((node.SyntaxTree as CSharpSyntaxTree)!, key);
        return node;
    }

    private static CSharpSyntaxTree StoreSyntaxTree(CSharpSyntaxTree tree, string? key = null)
    {
        SetSemanticModel(new[] { tree }, key ?? tree.ToString());
        return tree;
    }
}