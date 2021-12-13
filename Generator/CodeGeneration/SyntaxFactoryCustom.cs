using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.CodeGeneration
{
    public static class SyntaxFactoryCustom
    {
        public static LiteralExpressionSyntax LiteralExpressionCustom(object value) =>
            value switch
            {
                short n => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(n)),
                ushort n => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(n)),
                int n => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(n)),
                uint n => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(
                        n.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) + "U", n)),
                long n => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(
                        n.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) + "L", n)),
                ulong n => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(
                        n.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) + "UL", n)),
                float n => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(
                        ((float)n).ToString(System.Globalization.NumberFormatInfo.InvariantInfo) + "F", (float)n)),
                double n => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(
                        ((double)n).ToString(System.Globalization.NumberFormatInfo.InvariantInfo) + "D", (double)n)),
                decimal n => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(
                        n.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) + "M", n)),
                byte n => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(n)),
                char n => LiteralExpression(SyntaxKind.CharacterLiteralExpression, Literal(n)),
                string s => LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(s)),
                bool b => LiteralExpression(b ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression),
                _ => throw new ArgumentException($"Unhandled literal expression: '{value}'."),
            };

        public static ObjectCreationExpressionSyntax ConstructorCall(string name, IEnumerable<SyntaxNode> arguments)
            => ObjectCreationExpression(Token(ParseTrailingTrivia(""),
                    SyntaxKind.NewKeyword, ParseTrailingTrivia(" ")),
                    IdentifierName(name), ArgumentList(SeparatedList(arguments)), null);

        public static InvocationExpressionSyntax InvocationExpressionCustom(string name, IEnumerable<ExpressionSyntax> arguments)
            => InvocationExpressionCustom(IdentifierName(name), arguments);

        public static InvocationExpressionSyntax InvocationExpressionCustom(ExpressionSyntax expression, IEnumerable<ExpressionSyntax> arguments)
            => InvocationExpression(expression, ArgumentList(SeparatedList(arguments.Select(x => Argument(x)))));

        public static InvocationExpressionSyntax DottedInvocationExpressionCustom(string name, IEnumerable<ExpressionSyntax> arguments)
            => InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, arguments.First(), IdentifierName(name)),
                ArgumentList(SeparatedList(arguments?.Skip(1).Select(x => Argument(x)))));
    }
}