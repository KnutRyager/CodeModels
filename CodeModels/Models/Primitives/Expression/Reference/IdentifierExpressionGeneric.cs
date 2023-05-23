using System;
using System.Collections.Generic;
using CodeModels.Factory;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Invocation;

namespace CodeModels.Models.Primitives.Expression.Reference;

public record IdentifierExpressionGeneric<T>(string Name, ICodeModel? Model = null)
    : IdentifierExpression(Name, CodeModelFactory.Type<T>(), Model: Model)
{
    public static IdentifierExpressionGeneric<T> Create(string name, ICodeModel? model = null) 
        => new(name, model);

    public InvocationFromReflection GetInvocation<U>(System.Linq.Expressions.Expression<Func<T, Func<U>>> expression, IEnumerable<IExpression>? arguments = null) 
        => CodeModelsFromReflection.Invocation(expression, this, arguments);

    public InvocationFromReflection GetInvocation<U, V>(System.Linq.Expressions.Expression<Func<T, Func<U, V>>> expression, IEnumerable<IExpression>? arguments = null) 
        => CodeModelsFromReflection.Invocation(expression, this, arguments);

    public InvocationFromReflection GetInvocation(System.Linq.Expressions.Expression<Func<T, Delegate>> expression, IEnumerable<IExpression>? arguments = null) 
        => CodeModelsFromReflection.Invocation(expression, this, arguments);
}
