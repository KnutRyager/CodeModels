using System;
using System.Reflection;
using CodeModels.Models;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Reference;
using CodeModels.Reflection;
using Common.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Factory;

public static class CodeModelTypeParsing
{
    public static IType Parse(SyntaxToken token, SemanticModel? model = null) => Parse(token.ToString(), model);
    public static IType Type(string code) => Parse(code);
    public static IType Parse(string identifier, SemanticModel? model = null)
        => ParseType(ParseTypeName(identifier), model: model);

    public static IType ParseType(TypeSyntax? syntax, bool required = true, IType? knownType = null, SemanticModel? model = null)
    {
        var expression = Parse(syntax, knownType, model);
        return (expression is IType type ? (required ? type : type.ToOptionalType())
            : expression is IdentifierExpression identifier
                ? QuickType(required ? identifier.Name : $"{identifier.Name}?", type: ReflectionSerialization.IsShortHandName(identifier.Name)
                    ? ReflectionSerialization.DeserializeTypeLookAtShortNames(identifier.Name, !required)
                    : null)
                : expression.Get_Type()) ??
             //? Identifier(identifier.Name, symbol: SymbolUtils.GetSymbol(syntax, model), model: expression) : expression.Get_Type()) ??
             throw new NotImplementedException($"No type for: '{syntax}'.");
    }

    public static IExpression Parse(TypeSyntax? syntax, IType? knownType = null, SemanticModel? model = null) => syntax switch
    {
        ArrayTypeSyntax type => Parse(type, knownType, model),
        FunctionPointerTypeSyntax type => Parse(type, knownType, model),
        NameSyntax type => Parse(type, knownType, model),
        NullableTypeSyntax type => Parse(type, knownType, model),
        OmittedTypeArgumentSyntax type => Parse(type, model),
        PointerTypeSyntax type => Parse(type, knownType, model),
        PredefinedTypeSyntax type => Parse(type, knownType, model),
        RefTypeSyntax type => Parse(type, knownType, model),
        TupleTypeSyntax type => Parse(type, knownType, model),
        null => TypeShorthands.NullType,
        _ => throw new NotImplementedException($"TypeSyntax {syntax} not implemented.")
    };

    public static IType Parse(ArrayTypeSyntax syntax, IType? type = null, SemanticModel? model = null) => Models.QuickType.ArrayType(ParseType(syntax.ElementType, model: model));
    public static IType Parse(FunctionPointerTypeSyntax syntax, IType? type = null, SemanticModel? model = null) => throw new NotImplementedException();
    public static IExpression Parse(NameSyntax syntax, IType? type = null, SemanticModel? model = null) => syntax switch
    {
        AliasQualifiedNameSyntax name => Parse(name, type, model),
        QualifiedNameSyntax name => Parse(name, type, model),
        SimpleNameSyntax name => Parse(name, type, model),
        _ => throw new NotImplementedException($"NameSyntax {syntax} not implemented.")
    };
    public static IType Parse(AliasQualifiedNameSyntax syntax, IType? type = null, SemanticModel? model = null) => throw new NotImplementedException();
    public static IType Parse(QualifiedNameSyntax syntax, IType? type = null, SemanticModel? model = null)
        => TypeFromReflection.Create(model is null
            ? System.Type.GetType(syntax.ToString())
            : SemanticReflection.GetType(model.GetTypeInfo(syntax).Type));
    public static IExpression Parse(SimpleNameSyntax syntax, IType? type = null, SemanticModel? model = null) => syntax switch
    {
        GenericNameSyntax name => Parse(name, type, model),
        IdentifierNameSyntax name => Parse(name, type, model),
        _ => throw new NotImplementedException($"NameSyntax {syntax} not implemented.")
    };
    public static IType Parse(GenericNameSyntax syntax, IType? type = null, SemanticModel? model = null) => QuickType(syntax.ToString());
    public static IExpression Parse(IdentifierNameSyntax syntax, IType? type = null, SemanticModel? model = null) => QuickType(syntax.Identifier.ToString());
    public static IType Parse(NullableTypeSyntax syntax, IType? type = null, SemanticModel? model = null) => ParseType(syntax.ElementType, false);
    public static IType Parse(OmittedTypeArgumentSyntax syntax, SemanticModel? model = null) => throw new NotImplementedException();
    public static IType Parse(PointerTypeSyntax syntax, IType? type = null, SemanticModel? model = null) => throw new NotImplementedException();
    public static IType Parse(PredefinedTypeSyntax syntax, IType? type = null, SemanticModel? model = null)
        => QuickType(syntax.Keyword.ToString(), type: ReflectionSerialization.DeserializeTypeLookAtShortNames(syntax.Keyword.ToString()));
    public static IType Parse(RefTypeSyntax syntax, IType? type = null, SemanticModel? model = null) => throw new NotImplementedException();
    public static IType Parse(TupleTypeSyntax syntax, IType? type = null, SemanticModel? model = null) => throw new NotImplementedException();


}
