using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.CodeAnalysis;

namespace Generator.Test;

public static class TestUtil
{
    public static void CodeEqual<T1, T2>(this T1 node1, T2 node2, bool ignoreWhitespace = false) where T1 : SyntaxNode where T2 : SyntaxNode
        => CodeEqual(FormatSyntaxNode(node1, ignoreWhitespace), FormatSyntaxNode(node2, ignoreWhitespace));
    public static void CodeEqual<T>(this string str, T node, bool isExpression = false, bool ignoreWhitespace = false) where T : SyntaxNode
        => CodeEqual(isExpression ? str.ExpressionTree() : (SyntaxNode)str.Parse(), node, ignoreWhitespace);
    public static void CodeEqual<T>(this string str, IEnumerable<T> nodes) where T : SyntaxNode
    {
        if (nodes.Count() == 1) CodeEqual(str, nodes.First());
        else throw new ArgumentException($"Length of nodes not 1, it is '{nodes.Count()}'.");
    }
    public static void CodeEqual<T>(this T node, string str, bool isExpression = false, bool ignoreWhitespace = false) where T : SyntaxNode => CodeEqual(str, node, isExpression, ignoreWhitespace);
    public static void CodeEqual<T>(this IEnumerable<T> node, string str) where T : SyntaxNode => CodeEqual(str, node);
    public static void CodeEqual(this string str, ISymbol node) => CodeEqual(str, node?.ToString() ?? "");
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
        return (c == str.Length) ? -1 : c;
    }

    public static T FirstOrDefault<T>(this IEnumerable<T> enumerable, T defaultValue)
    {
        using var enumerator = enumerable.GetEnumerator();
        if (enumerator.MoveNext())
        {
            return enumerator.Current;
        }
        return defaultValue;
    }
}
