using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CodeModels.Execution.Context;
using CodeModels.Extensions;
using CodeModels.Factory;
using CodeModels.Models;
using CodeModels.Parsing;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TestCommon;

public static class TestUtil
{
    public static void CodeEqual<T1, T2>(this T1 node1, T2 node2, bool ignoreWhitespace = false) where T1 : SyntaxNode where T2 : SyntaxNode
        => FormatSyntaxNode(node1, ignoreWhitespace).CodeEqual(FormatSyntaxNode(node2, ignoreWhitespace));
    public static void CodeEqual<T>(this string str, T node, bool isExpression = false, bool ignoreWhitespace = false) where T : SyntaxNode
        => (isExpression ? str.ExpressionTree() : (SyntaxNode)str.Parse()).CodeEqual(node, ignoreWhitespace);
    public static void CodeEqual<T>(this string str, IEnumerable<T> nodes) where T : SyntaxNode
    {
        if (nodes.Count() == 1) str.CodeEqual(nodes.First());
        else throw new ArgumentException($"Length of nodes not 1, it is '{nodes.Count()}'.");
    }
    public static void CodeModelEqual<T>(this T codeModel, string str, bool isExpression = false, bool ignoreWhitespace = false) where T : ICodeModel => str.CodeEqual(codeModel.Syntax(), isExpression, ignoreWhitespace);
    public static void CodeEqual<T>(this T node, string str, bool isExpression = false, bool ignoreWhitespace = false) where T : SyntaxNode => str.CodeEqual(node, isExpression, ignoreWhitespace);
    public static void CodeEqual<T>(this IEnumerable<T> node, string str) where T : SyntaxNode => str.CodeEqual(node);
    public static void CodeEqual(this string str, ISymbol node) => str.CodeEqual(node?.ToString() ?? "");
    public static void CodeEqual(this string a, string b) => a.Should().Be(b);

    private static string FormatSyntaxNode(SyntaxNode node, bool ignoreWhitespace = false) => FixWhiteSpace(node.NormalizeWhitespace().ToString(), ignoreWhitespace);

    private static string FixWhiteSpace(string str, bool ignoreWhitespace = false)
    {
        var firstNonWhitespace = IndexOfFirstNonWhitespace(str);
        if (firstNonWhitespace == -1) return "";
        return ignoreWhitespace ? Regex.Replace(str, @"\s+", "") : $"\n{str[firstNonWhitespace..]}";
    }

    public static int IndexOfFirstNonWhitespace(string str)
    {
        int c = str.TakeWhile(c => char.IsWhiteSpace(c)).Count();
        return c == str.Length ? -1 : c;
    }

    public static string ParseAndRegenerateCode(this string str, string? key = null, SourceCodeKind kind = SourceCodeKind.Regular)
    {
        var model = str.ParseAndKeepSemanticModel(key, kind);
        var topLevelRewriter = new TopLevelStatementRewriter();
        var rewrittenCompilationUnit = (CompilationUnitSyntax)model.Compilation.GetVisit(topLevelRewriter);
        if (rewrittenCompilationUnit != model.Compilation)
        {
            model = rewrittenCompilationUnit.ToString().ParseAndKeepSemanticModel(key, kind);
        }
        var programContext = ProgramContext.NewContext(model.Compilation, model.Model);
        var parser = new CodeModelParser(model.Model, model.Compilation, programContext);
        var compilationModel = parser.Parse();
        var code = FormatSyntaxNode(compilationModel.Syntax());
        return code;
    }

    public static void AssertParsedAndGeneratedEqual(this string code)
    {
        var generatedCode = code.ParseAndRegenerateCode();
        generatedCode = FixNewlines(generatedCode);
        code = FixNewlines(code);
        generatedCode.Trim().Should().Be(code.Trim());
    }

    private static string FixNewlines(string s) => s.Replace("\r\n", Environment.NewLine).Replace("\n", Environment.NewLine);
}
