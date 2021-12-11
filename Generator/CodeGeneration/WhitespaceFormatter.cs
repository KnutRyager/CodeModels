using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.CodeGeneration
{
    public static class WhitespaceFormatter
    {
        public static T NormalizeWhitespacesSingleLineProperties<T>(this T node) where T : SyntaxNode =>
            (T)node.NormalizeWhitespace().SingleLineProperties();

        public static SyntaxNode SingleLineProperties(this SyntaxNode node) => new SingleLinePropertyRewriter().Visit(node);

        class SingleLinePropertyRewriter : CSharpSyntaxRewriter
        {
            public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node) =>
                node.NormalizeWhitespace(indentation: "", eol: " ")
                    .WithLeadingTrivia(node.GetLeadingTrivia())
                    .WithTrailingTrivia(node.GetTrailingTrivia());
        }
    }
}