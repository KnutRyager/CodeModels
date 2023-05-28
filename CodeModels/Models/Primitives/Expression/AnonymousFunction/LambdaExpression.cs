using System.Collections.Generic;
using CodeModels.Models;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Member;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator.Models.Primitives.Expression.AnonymousFunction;

public abstract record LambdaExpression<T>(
    Modifier Modifier,
    bool IsAsync,
    ParameterList Parameters,
    IType Type,
    Block? Body,
    IExpression? ExpressionBody)
    : AnonymousFunctionExpression<T>(Modifier, Type, Body, ExpressionBody)
    where T : LambdaExpressionSyntax
{
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Parameters;
        if (Body is not null) yield return Body;
        if (ExpressionBody is not null) yield return ExpressionBody;
        foreach (var child in base.Children()) yield return child;
    }
}