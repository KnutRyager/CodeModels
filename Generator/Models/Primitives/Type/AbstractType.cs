using System;
using Common.Reflection;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models
{
    public abstract record AbstractType(string Identifier, bool Required = true, bool IsMulti = false, TypeSyntax? Syntax = null, Type? Type = null) : IType
    {
        private Type? _cachedType;

        public static IType Parse(string code) => Parse(ParseTypeName(code));

        // TODO
        public static IType Parse(TypeSyntax? type, bool required = true, TypeSyntax? fullType = null) => type switch
        {
            PredefinedTypeSyntax t => new QuickType(t.Keyword.ToString(), required, Syntax: fullType ?? t),
            NullableTypeSyntax t => Parse(t.ElementType, false, fullType: fullType ?? t),
            IdentifierNameSyntax t => new QuickType(t.Identifier.ToString(), Syntax: fullType ?? t),
            ArrayTypeSyntax t => new QuickType(t.ElementType.ToString(), IsMulti: true, Syntax: fullType ?? t),
            GenericNameSyntax t => new QuickType(t.Identifier.ToString(), Syntax: fullType ?? t),
            null => TypeShorthands.NullType,
            _ => throw new ArgumentException($"Unhandled {nameof(TypeSyntax)}: '{type}'.")
        };

        public IType ToMultiType() => this with { IsMulti = true };

        public TypeSyntax TypeSyntax() => Syntax ?? TypeSyntaxNullableWrapped(TypeSyntaxMultiWrapped(TypeSyntaxUnwrapped()));
        public TypeSyntax TypeSyntaxNonMultiWrapped() => Syntax ?? TypeSyntaxNullableWrapped(TypeSyntaxUnwrapped());
        public TypeSyntax TypeSyntaxNullableWrapped(TypeSyntax type) => Required ? type : NullableType(type);
        public TypeSyntax TypeSyntaxMultiWrapped(TypeSyntax type) => IsMulti ? ArrayType(type, rankSpecifiers: List(new[] { ArrayRankSpecifierCustom() })) : type;

        public TypeSyntax TypeSyntaxUnwrapped() => Identifier switch
        {
            _ when TypeShorthands.PredefinedTypes.ContainsKey(Identifier) => PredefinedType(Token(TypeShorthands.PredefinedTypes[Identifier])),
            _ => IdentifierName(Identifier(Identifier))
        };

        public Type? GetReflectedType() => _cachedType ??= Type ??
            (ReflectionSerialization.IsShortHandName(Identifier) ? ReflectionSerialization.DeserializeTypeLookAtShortNames(Identifier) : default);

        public virtual string GetMostSpecificType() => Syntax?.ToString() ?? Identifier;
    }
}