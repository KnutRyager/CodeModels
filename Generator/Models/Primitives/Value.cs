using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using static CodeAnalyzation.CodeGeneration.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;


namespace CodeAnalyzation.Models
{
    public class Value
    {
        public static readonly Value NullValue = new(TType.NullType, null);
        public TType Type { get; set; }
        public ISymbol? Symbol { get; set; }
        public object? LiteralValue { get; set; }
        public string? SerializedValue { get; set; }
        public Property? Property { get; set; }

        public Value(TType type, object? literalValue)
        {
            Type = type;
            LiteralValue = literalValue;
            SerializedValue = JsonConvert.SerializeObject(literalValue);
        }

        public Value(Property property)
        {
            Property = property;
            Type = property.Type;
        }

        public Value(TType type, string? serializedValue)
        {
            Type = type;
            SerializedValue = serializedValue;
            var reflectedType = Type.GetReflectedType();
            if (reflectedType != null)
                LiteralValue = JsonConvert.DeserializeObject(SerializedValue!, reflectedType);
        }

        public Value(ISymbol symbol)
        {
            Symbol = symbol;
            Type = new TType(Symbol);   // TODO: Containing type for prop
        }

        public Value(EnumMemberDeclarationSyntax value) : this(new TType(typeof(string)), value.Identifier) { }

        public static Value FromValue(object? literalValue) => literalValue is null ? NullValue : new(new(literalValue.GetType()), literalValue);

        public LiteralExpressionSyntax? LiteralExpression => LiteralValue != null ? LiteralExpressionCustom(LiteralValue) : default;
        public ExpressionSyntax Expression => (ExpressionSyntax?)LiteralExpression ?? Property?.NameSyntax ?? (Symbol is not null ? IdentifierName(Symbol.Name) : default)!;

        public EnumMemberDeclarationSyntax ToEnumValue() => EnumMemberDeclaration(
                attributeLists: default,
                identifier: Type switch
                {
                    _ when Type.GetReflectedType() == typeof(string) && LiteralValue is string name => Identifier(name),
                    _ => throw new ArgumentException($"Unhandled enum name: '{LiteralValue}' of type '{Type}'.")
                },
                equalsValue: default!);

        public ArgumentSyntax ToArgument() => ArgumentCustom(
                nameColon: default,
                refKindKeyword: default,
                expression: Expression);
    }
}