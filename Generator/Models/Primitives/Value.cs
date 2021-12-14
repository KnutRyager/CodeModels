using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;


namespace CodeAnalyzation.Models
{
    public class Value
    {
        public TType Type { get; set; }
        public object? LiteralValue { get; set; }
        public string? SerializedValue { get; set; }

        public Value(TType type, object? literalValue)
        {
            Type = type;
            LiteralValue = literalValue;
            SerializedValue = JsonConvert.SerializeObject(literalValue);
        }

        public Value(TType type, string? serializedValue)
        {
            Type = type;
            SerializedValue = serializedValue;
            var reflectedType = Type.GetReflectedType();
            if (reflectedType != null)
                LiteralValue = JsonConvert.DeserializeObject(SerializedValue!, reflectedType);
        }

        public Value(EnumMemberDeclarationSyntax value) : this(new TType(typeof(string)), value.Identifier) { }

        public static Value FromValue(object literalValue) => new(new(literalValue.GetType()), literalValue);

        public EnumMemberDeclarationSyntax ToEnumValue() => EnumMemberDeclaration(
                attributeLists: default,
                identifier: Type switch
                {
                    _ when Type.GetReflectedType() == typeof(string) && LiteralValue is string name => Identifier(name),
                    _ => throw new ArgumentException($"Unhandled enum name: '{LiteralValue}' of type '{Type}'.")
                },
                equalsValue: default);
    }
}