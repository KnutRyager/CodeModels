using Microsoft.CodeAnalysis;
using Xunit;
namespace CodeAnalysisTests;

public static class TestUtil
{
    public static void CodeEqual(string str, SyntaxNode node) => Assert.Equal(str, node.ToString());
    public static void CodeEqual(string str, ISymbol node) => Assert.Equal(str, node.ToString());
}
