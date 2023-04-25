using System;
using System.Collections.Generic;
using CodeAnalyzation.Generation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public interface IFieldOrProperty : IMember, ICodeModel, ITypeModel
{
    ITypeDeclaration? Owner { get; set; }
    ExpressionSyntax? ExpressionSyntax { get; }
    IExpression Value { get; }
    IExpression ValueOrDefault();
    IExpression AccessValue(string identifier, IType? type = null, ISymbol? symbol = null);
    IExpression AccessValue(IExpression? instance = null);
    ExpressionSyntax? AccessSyntax(IExpression? instance = null);
}

public interface IFieldOrProperty<T> : IMember<T>, IFieldOrProperty where T : MemberDeclarationSyntax
{
}

public abstract record FieldOrProperty<T>(string Name, IType Type, List<AttributeList> Attributes, Modifier Modifier, IExpression Value)
    : NamedCodeModel<T>(Name), IFieldOrProperty<T> where T : MemberDeclarationSyntax
{
    public ITypeDeclaration? Owner { get; set; }
    public IType Get_Type() => Type;
    public virtual bool IsStatic => Modifier.HasFlag(Modifier.Static);
    public TypeSyntax TypeSyntax() => Get_Type().Syntax();
    public abstract T SyntaxWithModifiers(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None);
    public override T Syntax() => SyntaxWithModifiers();
    MemberDeclarationSyntax IMember.Syntax() => Syntax();

    public ExpressionSyntax? AccessSyntax(IExpression? instance = null) => Owner is null && instance is null ? NameSyntax()
        : SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, instance is null ? SyntaxFactory.IdentifierName(Owner!.Name)
            : SyntaxFactory.IdentifierName(instance.Syntax().ToString()), SyntaxFactory.Token(SyntaxKind.DotToken), NameSyntax());

    public abstract IExpression AccessValue(IExpression? instance = null);
    public IExpression AccessValue(string identifier, IType? type = null, ISymbol? symbol = null)
        => AccessValue(CodeModelFactory.Identifier(identifier, type, symbol));

    MemberDeclarationSyntax IMember.SyntaxWithModifiers(Modifier modifier, Modifier removeModifier)
        => SyntaxWithModifiers(modifier, removeModifier);

    public ParameterSyntax ToParameter() => SyntaxFactory.Parameter(
            attributeLists: default,
            modifiers: default,
            type: TypeSyntax(),
            identifier: ToIdentifier(),
            @default: Initializer());

    public TupleElementSyntax ToTupleElement()
        => SyntaxFactory.TupleElement(type: TypeSyntax(),
        identifier: SyntaxFactoryCustom.TupleNameIdentifier(Name));
    //identifier: SyntaxFactoryCustom.TupleNameIdentifier(IsRandomlyGeneratedName ? null : Name));

    public EqualsValueClauseSyntax? Initializer() => DefaultValueSyntax() switch
    {
        ExpressionSyntax expression => SyntaxFactory.EqualsValueClause(expression),
        _ => default
    };

    public ExpressionSyntax? DefaultValueSyntax() => ExpressionSyntax;

    public CodeModel<T> Render(Namespace @namespace)
        => throw new System.NotImplementedException();

    ICodeModel IMember.Render(Namespace @namespace)
        => Render(@namespace);

    public IType ToType()
    {
        throw new System.NotImplementedException();
    }

    public IExpression ToExpression()
    {
        throw new System.NotImplementedException();
    }

    public IExpression ValueOrDefault() => (Value is null || (LiteralExpression)Value == CodeModelFactory.VoidValue || (LiteralExpression)Value == CodeModelFactory.NullValue)
        && (Type.ReflectedType?.IsValueType ?? false)
        ? CodeModelFactory.Literal(Activator.CreateInstance(Type.ReflectedType)) : Value ?? CodeModelFactory.NullValue;

    public ExpressionSyntax? ExpressionSyntax => Value switch
    {
        _ when ReferenceEquals(Value, CodeModelFactory.VoidValue) => default,
        _ => Value.Syntax()
    };
}
