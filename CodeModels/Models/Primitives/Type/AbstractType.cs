using System;
using System.Collections.Generic;
using System.Linq;
using CodeModels.Execution.Context;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Reference;
using CodeModels.Models.Primitives.Member;
using CodeModels.Utils;
using Common.DataStructures;
using Common.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Generation.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static CodeModels.Factory.CodeModelFactory;
using Microsoft.CodeAnalysis.CSharp;
using CodeModels.Factory;

namespace CodeModels.Models;

public abstract record AbstractType(string TypeName, EqualityList<IType> GenericTypes, bool Required = true, bool IsMulti = false, Type? ReflectedType = null, ITypeSymbol? Symbol = null)

//public abstract record AbstractType(string TypeName, EqualityList<IType> GenericTypes, Type? ReflectedType = null, ITypeSymbol? Symbol = null)
    : CodeModel<TypeSyntax>, IType
{
    private Type? _cachedType;

    public abstract IType ToMultiType();
    public abstract IType ToOptionalType();
    //public IType ToMultiType() => this with { IsMulti = true };
    //public bool Required => TypeName.EndsWith("?");
    //public bool IsMulti => TypeName.EndsWith("[]");
    public string Name => $"{TypeName}{(PrintAsGeneric() ? "<" : "")}{(PrintAsGeneric() ? string.Join(",", GenericTypes.Select(x => x.Name)) : "")}{(PrintAsGeneric() ? ">" : "")}{(IsMulti ? "[]" : "")}{(Required ? "" : "?")}";
    public bool IsStatic => ReflectedType is not null && ReflectionUtil.IsStatic(ReflectedType);

    public override TypeSyntax Syntax() => TypeSyntaxNullableWrapped(TypeSyntaxMultiWrapped(TypeSyntaxUnwrapped()));
    public TypeSyntax TypeSyntaxNonMultiWrapped() => TypeSyntaxNullableWrapped(TypeSyntaxUnwrapped());
    public TypeSyntax TypeSyntaxNullableWrapped(TypeSyntax type) => Required ? type : NullableType(type);
    public TypeSyntax TypeSyntaxMultiWrapped(TypeSyntax type) => IsMulti ? ArrayType(type, rankSpecifiers: SyntaxFactory.List(new[] { ArrayRankSpecifierCustom() })) : type;

    public TypeSyntax TypeSyntaxUnwrapped() => Name switch
    {
        _ when TypeShorthands.PredefinedTypes.ContainsKey(NamespaceUtils.NamePart(TypeName)) => PredefinedType(Token(TypeShorthands.PredefinedTypes[NamespaceUtils.NamePart(TypeName)])),
        _ when PrintAsGeneric() => GenericName(Identifier(TypeName),
            TypeArgumentList(SeparatedList(GenericTypes.Select(x => x.Syntax())))),
        _ => IdentifierName(Identifier(TypeName))
    };

    public virtual Type? GetReflectedType() => _cachedType ??= ReflectedType ??
        (ReflectionSerialization.IsShortHandName(TypeName, true) ? ReflectionSerialization.DeserializeTypeLookAtShortNames(TypeName) : default);

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

    public Argument ToArgument() => throw new NotImplementedException();
    public ArgumentList ToArgumentList() => ToArgument().ToArgumentList();

    public ArgumentSyntax ToArgumentSyntax() => ToArgument().Syntax();
    public IExpression Evaluate(ICodeModelExecutionContext context) => this;
    public object? EvaluatePlain(ICodeModelExecutionContext context) => null;
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
    public bool Equals(IType other, ICodeModelExecutionContext context)
        => TypeName == other.TypeName; // TODO: Check assembly
    public bool IsAssignableFrom(IType other, ICodeModelExecutionContext context)
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

    public ParameterSyntax ToParameterSyntax()
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

    private bool PrintAsGeneric() => Required && GenericTypes.Count > 0 && !TypeName.StartsWith("Nullable") && (!ReflectedType?.Name.StartsWith("Nullable") ?? true);

    public Parameter ToParameter()
    {
        throw new NotImplementedException();
    }
    public ParameterList ToParameterList() => CodeModelFactory.ParamList(this);
}
