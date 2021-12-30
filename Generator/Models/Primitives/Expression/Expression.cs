using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.CodeGeneration.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;


namespace CodeAnalyzation.Models
{
    public class Expression : IExpression
    {
        public static readonly Expression NullValue = new LiteralExpression(TType.NullType);
        public TType Type { get; set; }
        public ISymbol? Symbol { get; set; }

        public Expression(TType type)
        {
            Type = type;
        }

        public Expression(ISymbol symbol)
        {
            Symbol = symbol;
            Type = new(Symbol);   // TODO: Containing type for prop
        }

        public static Expression FromSyntax(ExpressionSyntax? syntax) => syntax is null ? NullValue : new ExpressionFromSyntax(syntax);

        public virtual LiteralExpressionSyntax? LiteralSyntax => default;
        public virtual ExpressionSyntax Syntax => (ExpressionSyntax?)LiteralSyntax ?? (Symbol is not null ? IdentifierName(Symbol.Name) : this == NullValue ? IdentifierName("null") : throw new Exception("Expression has no syntax node or value."));

        public EnumMemberDeclarationSyntax ToEnumValue(int? value = null) => EnumMemberDeclaration(
                attributeLists: default,
                identifier: Type switch
                {
                    _ when Type.GetReflectedType() == typeof(string) && LiterallyValue is string name => Identifier(name),
                    _ => throw new ArgumentException($"Unhandled enum name: '{LiterallyValue}' of type '{Type}'.")
                },
                equalsValue: value is null ? default! : EqualsValueClause(LiteralExpressionCustom(value)));

        public ArgumentSyntax ToArgument() => ArgumentCustom(
                nameColon: default,
                refKindKeyword: default,
                expression: Syntax);


        public virtual object? LiterallyValue => null;
        public bool IsLiteralExpression => LiteralSyntax is not null;
    }
}