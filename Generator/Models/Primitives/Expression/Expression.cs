using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.CodeGeneration.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;


namespace CodeAnalyzation.Models
{
    public record Expression(TType Type, ISymbol? Symbol = null) : IExpression
    {
        public static readonly Expression NullValue = new LiteralExpression(TType.NullType);

        public Expression(TType type) : this(type, null) { }

        // TODO: Containing type for prop
        public Expression(ISymbol symbol) : this(new(symbol), symbol) { }

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