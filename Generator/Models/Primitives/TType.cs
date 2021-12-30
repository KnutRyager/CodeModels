using System;
using System.Collections.Generic;
using Common.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.CodeGeneration.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

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
        public bool IsMulti { get; set; }
        private readonly TypeSyntax? _typeSyntax;

        public TType(string identifier, bool required = true, bool isMulti = false, TypeSyntax? syntax = null)
        {
            Identifier = identifier;
            Required = required;
            IsMulti = isMulti;
            _typeSyntax = syntax;
        }

        public TType(ISymbol typeSymbol, bool required = true, bool isMulti = false, TypeSyntax? syntax = null)
        {
            Symbol = typeSymbol;
            Identifier = ReflectionSerialization.GetToShortHandName(Symbol.Name);
            Required = required;
            IsMulti = isMulti;
            _typeSyntax = syntax;
        }

        public TType(Type type, bool required = true, bool isMulti = false, TypeSyntax? syntax = null)
        {
            Type = type;
            Identifier = ReflectionSerialization.GetToShortHandName(type.Name);
            Required = required;
            IsMulti = isMulti;
            _typeSyntax = syntax;
        }

        public TType(TType other)
        {
            Symbol = other.Symbol;
            Type = other.Type;
            Identifier = other.Identifier;
            Required = other.Required;
            IsMulti = other.IsMulti;
            _typeSyntax = other._typeSyntax;
        }

        public static TType Parse(string code) => Parse(ParseTypeName(code));

        // TODO
        public static TType Parse(TypeSyntax type, bool required = true, TypeSyntax? fullType = null) => type switch
        {
            PredefinedTypeSyntax t => new TType(t, required, fullType: fullType ?? t),
            NullableTypeSyntax t => Parse(t.ElementType, false, fullType: fullType ?? t),
            IdentifierNameSyntax t => new TType(t.Identifier.ToString(), syntax: fullType ?? t),
            ArrayTypeSyntax t => new TType(t.ElementType.ToString(), isMulti: true, syntax: fullType ?? t),
            GenericNameSyntax t => new TType(t.Identifier.ToString(), syntax: fullType ?? t),
            _ => throw new ArgumentException($"Unhandled {nameof(TypeSyntax)}: '{type}'.")
        };

        //SyntaxNodeExtensions.GetSemanticModel(type.SyntaxTree).GetDeclaredSymbol(type)) { }
        public TType(PredefinedTypeSyntax type, bool required = true, TypeSyntax? fullType = null) : this(type.Keyword.ToString(), required, syntax: fullType ?? type) { }

        public static TType MultiType(TType type)
        {
            var copy = new TType(type)
            {
                IsMulti = true
            };
            return copy;
        }

        public TypeSyntax TypeSyntax() => _typeSyntax ?? TypeSyntaxNullableWrapped(TypeSyntaxMultiWrapped(TypeSyntaxUnwrapped()));
        public TypeSyntax TypeSyntaxNonMultiWrapped() => _typeSyntax ?? TypeSyntaxNullableWrapped(TypeSyntaxUnwrapped());
        public TypeSyntax TypeSyntaxNullableWrapped(TypeSyntax type) => Required ? type : NullableType(type);
        public TypeSyntax TypeSyntaxMultiWrapped(TypeSyntax type) => IsMulti ? ArrayType(type, rankSpecifiers: List(new[] { ArrayRankSpecifierCustom() })) : type;

        public TypeSyntax TypeSyntaxUnwrapped() => Identifier switch
        {
            _ when _predefinedTypes.ContainsKey(Identifier) => PredefinedType(Token(_predefinedTypes[Identifier])),
            _ => IdentifierName(Identifier(Identifier))
        };

        public Type? GetReflectedType() => Type ??= ReflectionSerialization.IsShortHandName(Identifier) ? ReflectionSerialization.DeserializeTypeLookAtShortNames(Identifier) : default;

        public string GetMostSpecificType()
        {
            return _typeSyntax?.ToString() ?? Symbol?.ToString() ?? Identifier;
        }

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

        public override string ToString() => $"{Identifier}: Symbol: {Symbol}, Type: {Type}, Required: {Required}, IsMulti: {IsMulti}";
    }
}