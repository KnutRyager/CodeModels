using System;
using System.Collections.Generic;
using CodeModels.Factory;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Access;
using CodeModels.Models.Primitives.Expression.Invocation;
using CodeModels.Models.Primitives.Member;

namespace CodeModels.Models.Primitives.Expression.Reference;

public record IdentifierExpressionGeneric<T>(string Name, ICodeModel? Model = null)
    : IdentifierExpression(Name, CodeModelFactory.Type<T>(), Model: Model)
{
    public static IdentifierExpressionGeneric<T> Create(string name, ICodeModel? model = null)
        => new(name, model);

    //public PropertyExpressionFromReflection GetPropertyAccess(System.Linq.Expressions.Expression<Func<T, Delegate>> expression) 
    //    => CodeModelsFromReflection.PropertyAccess(expression, this);
    public MemberAccessExpression Access(System.Linq.Expressions.Expression<Func<T, object>> expression)
        => CodeModelFactory.MemberAccess(this, CodeModelsFromReflection.GetIdentifier(expression));
    public PropertyExpressionFromReflection GetPropertyAccess(System.Linq.Expressions.Expression<Func<T, object>> expression)
        => CodeModelsFromReflection.PropertyAccess(expression, this);
    //public PropertyExpressionFromReflection GetPropertyAccess(System.Linq.Expressions.Expression<Action<T>> expression) 
    //    => CodeModelsFromReflection.PropertyAccess(expression, this);

    public InvocationFromReflection GetInvocation(System.Linq.Expressions.Expression<Func<T, Delegate>> expression, IEnumerable<IExpression>? arguments = null)
        => CodeModelsFromReflection.Invocation(expression, this, arguments);
    public InvocationFromReflection GetInvocation(System.Linq.Expressions.Expression<Func<T, object>> expression, IEnumerable<IExpression>? arguments = null)
        => CodeModelsFromReflection.Invocation(expression, this, arguments);
    public IExpression GetModel(System.Linq.Expressions.Expression<Func<T, object>> expression, IEnumerable<IExpression>? arguments = null)
        => CodeModelsFromReflection.GetModel(expression).ExpressionBody switch
        {
            InvocationFromReflection invocation => CodeModelsFromReflection.Invocation(invocation.Method, invocation.Caller, arguments ?? invocation.Arguments),
            _ => throw new NotImplementedException()
        };

    public InvocationFromReflection GetInvocation(System.Linq.Expressions.Expression<Action<T>> expression, IEnumerable<IExpression>? arguments = null)
        => CodeModelsFromReflection.Invocation(expression, this, arguments);
    public InvocationFromReflection GetInvocation(System.Linq.Expressions.Expression<Func<T, Delegate>> expression, params IExpression[] arguments)
        => CodeModelsFromReflection.Invocation(expression, this, arguments);
    public InvocationFromReflection GetInvocation(System.Linq.Expressions.Expression<Func<T, object>> expression, params IExpression[] arguments)
        => CodeModelsFromReflection.Invocation(expression, this, arguments);
    public InvocationFromReflection GetInvocation(System.Linq.Expressions.Expression<Action<T>> expression, params IExpression[] arguments)
        => CodeModelsFromReflection.Invocation(expression, this, arguments);

    //public IExpression GetAny(System.Linq.Expressions.Expression<Func<T, object>> expression) 
    //    => InvocationExpression.Create(this, CodeModelsFromReflection.GetModel(expression));
}
