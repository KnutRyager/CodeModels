using System;
using System.Collections.Generic;
using System.Linq;
using Common.DataStructures;
using Common.Reflection;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models;

public abstract record AbstractType(string Identifier, EqualityList<IType> GenericTypes, bool Required = true, bool IsMulti = false, Type? ReflectedType = null)
    : CodeModel<TypeSyntax>, IType
{
    private Type? _cachedType;

    public IType ToMultiType() => this with { IsMulti = true };
    public string Name => $"{Identifier}{(GenericTypes.Count > 0 ? "<" : "")}{string.Join(",", GenericTypes.Select(x => x.Name))}{(GenericTypes.Count > 0 ? ">" : "")}{(IsMulti ? "[]" : "")}";
    public bool IsStatic => ReflectedType is not null && ReflectionUtil.IsStatic(ReflectedType);

    public override TypeSyntax Syntax() => TypeSyntaxNullableWrapped(TypeSyntaxMultiWrapped(TypeSyntaxUnwrapped()));
    public TypeSyntax TypeSyntaxNonMultiWrapped() => TypeSyntaxNullableWrapped(TypeSyntaxUnwrapped());
    public TypeSyntax TypeSyntaxNullableWrapped(TypeSyntax type) => Required ? type : NullableType(type);
    public TypeSyntax TypeSyntaxMultiWrapped(TypeSyntax type) => IsMulti ? ArrayType(type, rankSpecifiers: List(new[] { ArrayRankSpecifierCustom() })) : type;

    public TypeSyntax TypeSyntaxUnwrapped() => Identifier switch
    {
        _ when TypeShorthands.PredefinedTypes.ContainsKey(Identifier) => PredefinedType(Token(TypeShorthands.PredefinedTypes[Identifier])),
        _ when GenericTypes.Count > 0 => GenericName(Identifier(Identifier),
            TypeArgumentList(SeparatedList(GenericTypes.Select(x => x.Syntax())))),
        _ => IdentifierName(Identifier(Identifier))
    };

    public virtual Type? GetReflectedType() => _cachedType ??= ReflectedType ??
        (ReflectionSerialization.IsShortHandName(Identifier) ? ReflectionSerialization.DeserializeTypeLookAtShortNames(Identifier) : default);

    public virtual string GetMostSpecificType() => Name;

    public override IEnumerable<ICodeModel> Children() => Array.Empty<ICodeModel>();

    public TypeParameterSyntax ToTypeParameter() => TypeParameter(Name);

    public IType GetGenericType(int index) => GenericTypes[index];

    public IType Get_Type() => this;
    public bool IsLiteralExpression => false;
    public LiteralExpressionSyntax? LiteralSyntax => null;
    public object? LiteralValue => null;

    public Modifier Modifier => throw new NotImplementedException();

    public ArgumentSyntax ToArgument() => throw new NotImplementedException();
    public IExpression Evaluate(IProgramModelExecutionContext context) => throw new NotImplementedException();
    public object? EvaluatePlain(IProgramModelExecutionContext context) => null;
    public EnumMemberDeclarationSyntax ToEnumValue(int? value = null) => throw new NotImplementedException();
    public ExpressionStatement AsStatement() => throw new NotImplementedException();
    ExpressionSyntax IExpression.Syntax() => Syntax();
    public IdentifierExpression GetIdentifier() => new(Get_Type().Name, Get_Type());

    MemberDeclarationSyntax IMember.Syntax()
    {
        throw new NotImplementedException();
    }

    public MemberDeclarationSyntax SyntaxWithModifiers(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None)
    {
        throw new NotImplementedException();
    }

    public TypeSyntax TypeSyntax()
    {
        throw new NotImplementedException();
    }
}
