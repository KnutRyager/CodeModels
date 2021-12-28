using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Microsoft.CodeAnalysis.CSharp;
using static CodeAnalyzation.CodeGeneration.SyntaxFactoryCustom;
using System.Collections.Generic;
using System;
using Common.Reflection;

namespace CodeAnalyzation.Models
{
    public class TType
    {
        private readonly IDictionary<string, SyntaxKind> _predefinedTypes = new Dictionary<string, SyntaxKind>()
        {
            { SyntaxKind.ByteKeyword.ToString(), SyntaxKind.ByteKeyword },
            { SyntaxKind.SByteKeyword.ToString(), SyntaxKind.SByteKeyword },
            { SyntaxKind.ShortKeyword.ToString(), SyntaxKind.ShortKeyword },
            { SyntaxKind.UShortKeyword.ToString(), SyntaxKind.UShortKeyword },
            { SyntaxKind.IntKeyword.ToString(), SyntaxKind.IntKeyword },
            { SyntaxKind.UIntKeyword.ToString(), SyntaxKind.UIntKeyword },
            { SyntaxKind.LongKeyword.ToString(), SyntaxKind.LongKeyword },
            { SyntaxKind.ULongKeyword.ToString(), SyntaxKind.ULongKeyword },
            { SyntaxKind.FloatKeyword.ToString(), SyntaxKind.FloatKeyword },
            { SyntaxKind.DoubleKeyword.ToString(), SyntaxKind.DoubleKeyword },
            { SyntaxKind.DecimalKeyword.ToString(), SyntaxKind.DecimalKeyword },
            { SyntaxKind.StringKeyword.ToString(), SyntaxKind.StringKeyword },
            { SyntaxKind.BoolKeyword.ToString(), SyntaxKind.BoolKeyword },
            { SyntaxKind.VoidKeyword.ToString(), SyntaxKind.VoidKeyword },
        };

        public string Identifier { get; set; }
        public ISymbol? Symbol { get; set; }
        public Type? Type { get; set; }
        public bool Required { get; set; }

        public TType(string identifier, bool required = true)
        {
            Identifier = identifier;
            Required = required;
        }

        public TType(ISymbol typeSymbol, bool required = true)
        {
            Symbol = typeSymbol;
            Identifier = ReflectionSerialization.GetToShortHandName(Symbol.Name);
            Required = required;
        }

        public TType(Type type, bool required = true)
        {
            Type = type;
            Identifier = ReflectionSerialization.GetToShortHandName(type.Name);
            Required = required;
        }

        public static TType Parse(string code) => Parse(ParseTypeName(code));

        public static TType Parse(TypeSyntax type, bool required = true) => type switch
        {
            PredefinedTypeSyntax t => new TType(t, required),
            NullableTypeSyntax t => Parse(t.ElementType, false),
            IdentifierNameSyntax t => new TType(t.Identifier.ToString()),
            ArrayTypeSyntax t => new TType(t.ElementType.ToString()),
            GenericNameSyntax t => new TType(t.Identifier.ToString()),
            _ => throw new ArgumentException($"Unhandled {nameof(TypeSyntax)}: '{type}'.")
        };

        //SyntaxNodeExtensions.GetSemanticModel(type.SyntaxTree).GetDeclaredSymbol(type)) { }
        public TType(PredefinedTypeSyntax type, bool required = true) : this(type.Keyword.ToString(), required) { }

        public TypeSyntax TypeSyntax() => Required ? TypeSyntaxNonNullableWrapped() : NullableType(TypeSyntaxNonNullableWrapped());

        public TypeSyntax TypeSyntaxNonNullableWrapped() => Identifier switch
        {
            _ when _predefinedTypes.ContainsKey(Identifier) => PredefinedType(Token(_predefinedTypes[Identifier])),
            _ => IdentifierName(Identifier(Identifier))
        };

        public Type? GetReflectedType() => Type ??= ReflectionSerialization.IsShortHandName(Identifier) ? ReflectionSerialization.DeserializeTypeLookAtShortNames(Identifier) : default;

        public override bool Equals(object obj)
        {
            var item = obj as TType;
            return item is not null 
                && Identifier.Equals(item.Identifier)
                && (Symbol?.Equals(item.Symbol, SymbolEqualityComparer.Default) ?? item.Symbol is null)
                && (Type?.Equals(item.Type) ?? item.Type is null)
                && Required.Equals(item.Required);
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 23 + Identifier.GetHashCode();
                hash = hash * 23 + Symbol?.GetHashCode() ?? 0;
                hash = hash * 23 + Type?.GetHashCode() ?? 0;
                hash = hash * 23 + Required.GetHashCode();
                return hash;
            }
        }
    }
}