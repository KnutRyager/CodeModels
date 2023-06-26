using System;
using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Factory;
using CodeModels.Models.Primitives.Expression.Access;
using CodeModels.Models.Primitives.Expression.Invocation;
using CodeModels.Models.Primitives.Expression.Reference;
using CodeModels.Models.Primitives.Member;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Generation.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;


namespace CodeModels.Models.Primitives.Expression.Abstract;

public abstract record Expression<T>(IType Type, ISymbol? Symbol = null, string? Name = null)
    : NamedCodeModel<T>(Name ?? Type.Name), IExpression<T>
    where T : ExpressionSyntax
{
    public Expression(IType type) : this(type, null) { }

    // TODO: Containing type for prop
    public Expression(ITypeSymbol symbol) : this(TypeFromSymbol.Create(symbol), symbol) { }

    public EnumMemberDeclarationSyntax ToEnumValue(int? value = null) => EnumMemberDeclaration(
            attributeLists: default,
            identifier: Type switch
            {
                _ when Type.GetReflectedType() == typeof(string) && LiteralValue() is string name => Identifier(name),
                _ => throw new ArgumentException($"Unhandled enum name: '{LiteralValue()}' of type '{Type}'.")
            },
            equalsValue: value is null ? default! : EqualsValueClause(LiteralExpressionCustom(value)));

    public virtual Argument ToArgument() => CodeModelFactory.Arg(this);
    public virtual ArgumentList ToArgumentList() => ToArgument().ToArgumentList();
    public ArgumentSyntax ToArgumentSyntax() => ToArgument().Syntax();

    public virtual object? LiteralValue() => null;
    public bool IsLiteralExpression => LiteralSyntax() is not null;
    public virtual LiteralExpressionSyntax? LiteralSyntax() => default;
    public IType Get_Type() => Type;
    public TypeSyntax TypeSyntax() => Get_Type().Syntax();

    ExpressionSyntax IExpression.Syntax() => Syntax();
    ExpressionOrPatternSyntax IExpressionOrPattern.Syntax() => Syntax();
    protected ExpressionSyntax PlanBSyntax() => (ExpressionSyntax?)LiteralSyntax() ?? (Symbol is not null ? IdentifierName(Symbol.Name) : ReferenceEquals(this, CodeModelFactory.NullValue) ? IdentifierName("null") : throw new Exception("Expression has no syntax node or value."));

    public abstract IExpression Evaluate(ICodeModelExecutionContext context);
    public virtual object? EvaluatePlain(ICodeModelExecutionContext context) => Evaluate(context).LiteralValue();
    public ExpressionStatement AsStatement() => new(this);
    public new virtual IdentifierExpression ToIdentifierExpression() => new(Type.Name, Type, Symbol: Symbol, Model: this);

    public InvocationFromReflection GetInvocation<U>(System.Linq.Expressions.Expression<Func<U, Delegate>> expression, IEnumerable<IExpression>? arguments = null)
        => CodeModelsFromReflection.Invocation(expression, this, arguments);

    public InvocationFromReflection GetInvocation<U>(System.Linq.Expressions.Expression<Func<U, object>> expression, IEnumerable<IExpression>? arguments = null)
        => CodeModelsFromReflection.Invocation(expression, this, arguments);

    public InvocationFromReflection GetInvocation<U>(System.Linq.Expressions.Expression<Action<U>> expression, IEnumerable<IExpression>? arguments = null)
        => CodeModelsFromReflection.Invocation(expression, this, arguments);
    public InvocationFromReflection GetInvocation(System.Linq.Expressions.Expression<Action<object>> expression, IEnumerable<IExpression>? arguments = null)
        => CodeModelsFromReflection.Invocation(expression, this, arguments);

    public InvocationFromReflection GetInvocation<U>(string name, IEnumerable<IExpression>? arguments = null)
        => GetInvocation(typeof(U), name, arguments);
    public InvocationFromReflection GetInvocation<U>(string name, params IExpression[] arguments)
        => GetInvocation(typeof(U), name, arguments);

    public InvocationFromReflection GetInvocation(Type type, string name, IEnumerable<IExpression>? arguments = null)
        => CodeModelsFromReflection.Invocation(type, name, this, arguments);

    public MemberAccessExpression Access(IdentifierExpression identifier) => CodeModelFactory.MemberAccess(this, identifier);
}
