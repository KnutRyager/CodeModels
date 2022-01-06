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

        public IType ToMultiType() => this with { IsMulti = true };
        public string Name => Identifier;
        public bool IsStatic => Type is not null && ReflectionUtil.IsStatic(Type);

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