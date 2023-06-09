﻿using System.Collections.Generic;
using System.Linq;
using CodeModels.Compilation;
using CodeModels.Factory;
using CodeModels.Models.Primitives.Expression.Abstract;
using Common.Files;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Extensions.SyntaxNodeExtensions;

namespace CodeModels.Parsing;

public static class ParseUtil
{
    public static SyntaxTree SyntaxTree(this string str, string? key = null, SourceCodeKind kind = SourceCodeKind.Regular) => str.Parse(key, kind).SyntaxTree;
    public static ExpressionSyntax ExpressionTree(this string str, string? key = null) => SyntaxFactory.ParseExpression(str);
    public static BracketedParameterListSyntax BracketedParameterList(this string str, string? key = null) => SyntaxFactory.ParseBracketedParameterList(str);

    public static SyntaxTree[] SyntaxTrees(this string str, string? key = null) => new[] { str.Parse(key).SyntaxTree };

    public static CSharpSyntaxTree ParseTree(this string str, SourceCodeKind kind = SourceCodeKind.Regular)
        => (CSharpSyntaxTree.ParseText(Mustache.RemoveMustache(str), options: new CSharpParseOptions(kind: kind)) as CSharpSyntaxTree)!;

    public static CompilationUnitSyntax Parse(this string str, string? key = null, SourceCodeKind kind = SourceCodeKind.Regular)
    {
        var tree = ParseTree(str, kind);
        return CompilationHandling.StoreSyntaxTree(tree, key ?? str).GetCompilationUnitRoot();
    }

    public static (CompilationUnitSyntax Compilation, SemanticModel Model) ParseAndKeepSemanticModel(this string str, string? key = null, SourceCodeKind kind = SourceCodeKind.Regular)
        => (Compilation: str.Parse(key, kind), GetSemanticModel(key: str));

    public static IExpression ParseExpression(this string code) => CodeModelParsing.ParseExpression(code);

    public static IEnumerable<CompilationUnitSyntax> Parse(this IEnumerable<string> strs, string? key = null)
    {
        var trees = strs.Select(x => ParseTree(x)).ToArray();
        SetSemanticModel(trees, key ?? string.Join(", ", strs));
        return trees.Select(tree => (CompilationUnitSyntax)tree.GetRoot());
    }

    public static IEnumerable<CompilationUnitSyntax> ParsePath(this string path) => FileUtil.ReadFilesToText(FileUtil.GetFiles(path)).Parse();

    private static void SetSemanticModel(IEnumerable<SyntaxTree> trees, string? key = null)
    {
        var compilationWithModel = CSharpCompilation.Create("MyCompilation",
            syntaxTrees: trees, references: Libraries.StandardSystemLibraries);
        //Note that we must specify the tree for which we want the model.
        //Each tree has its own semantic model
        SetCompilation(compilationWithModel, trees, key);
    }

    public static CompilationUnitSyntax ParseFile(this string path)
        => FileUtil.ReadFileToText(path).Parse();

    public static string SingleQuote(this string str) => str.Replace("\"", "'");

    //public static string ParseToJson(this string str)
    //{
    //    var parsed = str.Parse();
    //    var @namespace = parsed.GetNamespaces().FirstOrDefault();
    //    var classes = parsed.GetClasses().Select(x => x.ParseToJson(@namespace));
    //    return JsonList(classes);
    //}

    //public static string ParseToJson(this ClassDeclarationSyntax c, NamespaceDeclarationSyntax? @namespace) => ClassModel.Parse(c, @namespace).ToJson();
    //public static string ParseFileToJson(this string path) => FileUtil.ReadFileToText(path).ParseToJson();
    //public static string JsonList(IEnumerable<string> jsonObjects) => $"[{string.Join(",", jsonObjects)}]";
}
