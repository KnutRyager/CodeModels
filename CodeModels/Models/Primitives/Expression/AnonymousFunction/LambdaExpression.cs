using System.Collections.Generic;
using CodeModels.Models;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator.Models.Primitives.Expression.AnonymousFunction;

public abstract record LambdaExpression<T>(
    Modifier Modifier,
    bool IsAsync,
    NamedValueCollection Parameters,
    IType Type,
    Block? Body,
    IExpression? ExpressionBody)
    : AnonymousFunctionExpression<T>(Modifier, Type, Body, ExpressionBody)
    where T : LambdaExpressionSyntax
{
    public override IEnumerable<ICodeModel> Children()
    {
        foreach (var parameter in Parameters.Properties) yield return parameter;
        if (Body is not null) yield return Body;
        if (ExpressionBody is not null) yield return ExpressionBody;
        foreach (var child in base.Children()) yield return child;
    }
}