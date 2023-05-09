﻿using System;
using System.Collections.Generic;
using System.Linq;
using CodeModels.Utils;
using Common.DataStructures;
using Common.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Generation.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models;

public abstract record AbstractType(string TypeName, EqualityList<IType> GenericTypes, bool Required = true, bool IsMulti = false, Type? ReflectedType = null, ITypeSymbol? Symbol = null)
    : CodeModel<TypeSyntax>, IType
{
    private Type? _cachedType;

    public IType ToMultiType() => this with { IsMulti = true };
    public string Name => $"{TypeName}{(GenericTypes.Count > 0 ? "<" : "")}{string.Join(",", GenericTypes.Select(x => x.Name))}{(GenericTypes.Count > 0 ? ">" : "")}{(IsMulti ? "[]" : "")}";
    public bool IsStatic => ReflectedType is not null && ReflectionUtil.IsStatic(ReflectedType);

    public override TypeSyntax Syntax() => TypeSyntaxNullableWrapped(TypeSyntaxMultiWrapped(TypeSyntaxUnwrapped()));
    public TypeSyntax TypeSyntaxNonMultiWrapped() => TypeSyntaxNullableWrapped(TypeSyntaxUnwrapped());
    public TypeSyntax TypeSyntaxNullableWrapped(TypeSyntax type) => Required ? type : NullableType(type);
    public TypeSyntax TypeSyntaxMultiWrapped(TypeSyntax type) => IsMulti ? ArrayType(type, rankSpecifiers: List(new[] { ArrayRankSpecifierCustom() })) : type;

    public TypeSyntax TypeSyntaxUnwrapped() => Name switch
    {
        _ when TypeShorthands.PredefinedTypes.ContainsKey(NamespaceUtils.NamePart(TypeName)) => PredefinedType(Token(TypeShorthands.PredefinedTypes[NamespaceUtils.NamePart(TypeName)])),
        _ when GenericTypes.Count > 0 => GenericName(SyntaxFactory.Identifier(TypeName),
            TypeArgumentList(SeparatedList(GenericTypes.Select(x => x.Syntax())))),
        _ => IdentifierName(SyntaxFactory.Identifier(TypeName))
    };

    public virtual Type? GetReflectedType() => _cachedType ??= ReflectedType ??
        (ReflectionSerialization.IsShortHandName(TypeName) ? ReflectionSerialization.DeserializeTypeLookAtShortNames(TypeName) : default);

    public virtual string GetMostSpecificType() => TypeName;

    public override IEnumerable<ICodeModel> Children() => Array.Empty<ICodeModel>();

    public TypeParameterSyntax ToTypeParameter() => TypeParameter(Name);

    public IType GetGenericType(int index) => GenericTypes[index];

    public IType Get_Type() => this;
    public bool IsLiteralExpression => false;
    public LiteralExpressionSyntax? LiteralSyntax() => null;
    public object? LiteralValue() => null;

    public Modifier Modifier => Modifier.None;

    public abstract IType PlainType();

    public SimpleNameSyntax NameSyntax() => IdentifierName(Name);

    public ArgumentSyntax ToArgument() => throw new NotImplementedException();
    public IExpression Evaluate(IProgramModelExecutionContext context) => this;
    public object? EvaluatePlain(IProgramModelExecutionContext context) => null;
    public EnumMemberDeclarationSyntax ToEnumValue(int? value = null) => throw new NotImplementedException();
    public ExpressionStatement AsStatement() => throw new NotImplementedException();
    ExpressionSyntax IExpression.Syntax() => Syntax();
    ExpressionOrPatternSyntax IExpressionOrPattern.Syntax() => Syntax();
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
    public bool Equals(IType other, IProgramModelExecutionContext context)
        => TypeName == other.TypeName; // TODO: Check assembly
    public bool IsAssignableFrom(IType other, IProgramModelExecutionContext context)
        => (ReflectedType is Type type && other.ReflectedType is Type otherType
            && type.IsAssignableFrom(otherType)) || Equals(other, context); // TODO: Check for non-reflected

    public ICodeModel Render(Namespace @namespace)
    {
        throw new NotImplementedException();
    }

    public IType ToType()
    {
        throw new NotImplementedException();
    }

    public IExpression ToExpression()
    {
        throw new NotImplementedException();
    }

    public ParameterSyntax ToParameter()
    {
        throw new NotImplementedException();
    }

    public TupleElementSyntax ToTupleElement()
    {
        throw new NotImplementedException();
    }

    public IdentifierNameSyntax IdentifierNameSyntax()
        => ToIdentifierExpression().Syntax();

    public Microsoft.CodeAnalysis.SyntaxToken IdentifierSyntax()
        => ToIdentifierExpression().ToToken();

    public IdentifierExpression ToIdentifierExpression()
        => new(Name, Model: this, Symbol: Symbol);
}
