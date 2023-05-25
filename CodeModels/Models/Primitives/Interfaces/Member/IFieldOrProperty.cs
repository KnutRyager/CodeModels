using System;
using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Execution.Scope;
using CodeModels.Factory;
using CodeModels.Generation;
using CodeModels.Models.Interfaces;
using CodeModels.Models.Primitives.Attribute;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Reference;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public interface IFieldOrProperty : IMember, ICodeModel, ITypeModel, IAssigner, IInvokable
{
    IBaseTypeDeclaration? Owner { get; set; }
    ExpressionSyntax? ExpressionSyntax { get; }
    IExpression? Value { get; }
    IExpression ValueOrDefault();
    IInvocation AccessValue(string identifier, IType? type = null, ISymbol? symbol = null);
    IInvocation AccessValue(IExpression? instance = null);
    public abstract IExpression EvaluateAccess(IExpression? expression, ICodeModelExecutionContext context);

    ExpressionSyntax? AccessSyntax(IExpression? instance = null);
}

public interface IFieldOrProperty<T> : IMember<T>, IFieldOrProperty where T : MemberDeclarationSyntax
{
}

public abstract record FieldOrProperty<T>(string Name, IType Type, List<AttributeList> Attributes, Modifier Modifier, IExpression? Value)
    : NamedCodeModel<T>(Name),
    IFieldOrProperty<T>,
    IScopeHolder
    where T : MemberDeclarationSyntax
{
    public IBaseTypeDeclaration? Owner { get; set; }
    public IType Get_Type() => Type;
    public virtual bool IsStatic => Modifier.HasFlag(Modifier.Static);
    public TypeSyntax TypeSyntax() => Get_Type().Syntax();
    public abstract T SyntaxWithModifiers(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None);
    public override T Syntax() => SyntaxWithModifiers();
    MemberDeclarationSyntax IMember.Syntax() => Syntax();

    public ExpressionSyntax? AccessSyntax(IExpression? instance = null) => Owner is null && instance is null ? NameSyntax()
        : SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, instance is null ? SyntaxFactory.IdentifierName(Owner!.Name)
            : SyntaxFactory.IdentifierName(instance.Syntax().ToString()), SyntaxFactory.Token(SyntaxKind.DotToken), NameSyntax());

    public abstract IInvocation AccessValue(IExpression? instance = null);
    public IInvocation AccessValue(string identifier, IType? type = null, ISymbol? symbol = null)
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

    public IExpression ValueOrDefault() => (Value is null
            || (LiteralExpression)Value == CodeModelFactory.VoidValue
            || (LiteralExpression)Value == CodeModelFactory.NullValue)
        && (Value is not null && (Type.ReflectedType?.IsValueType ?? false))
        ? CodeModelFactory.Literal(Activator.CreateInstance(Type.ReflectedType)) : Value ?? CodeModelFactory.NullValue;

    public abstract IExpression EvaluateAccess(IExpression? expression, ICodeModelExecutionContext context);

    public abstract AssignmentExpression Assign(IExpression? instance, IExpression value);
    public abstract void Assign(IExpression? instance, IExpression value, ICodeModelExecutionContext context);
    public abstract void Assign(IExpression value, ICodeModelExecutionContext context, IList<ICodeModelExecutionScope> scopes);
    public abstract IInvocation Invoke(IExpression? caller, IEnumerable<IExpression> arguments);

    public ExpressionSyntax? ExpressionSyntax => Value switch
    {
        _ when Value is null || ReferenceEquals(Value, CodeModelFactory.VoidValue) => default,
        _ => Value.Syntax()
    };

    public IList<ICodeModelExecutionScope> GetScopes(IExpression? expression = null) => this switch
    {
        _ when expression is IInstantiatedObject instance => instance.GetScopes(null!),
        _ when Owner is IScopeHolder scopeHolder => scopeHolder.GetScopes(null!),
        _ when expression is IdentifierExpression identifier && identifier.Model is IScopeHolder staticReference => staticReference.GetScopes(null!),
        _ => throw new NotImplementedException()
    };

    public IList<ICodeModelExecutionScope> GetScopes(ICodeModelExecutionContext context) => this switch
    {
        _ when Owner is IScopeHolder scopeHolder => scopeHolder.GetScopes(context),
        _ => throw new NotImplementedException()
    };
}
