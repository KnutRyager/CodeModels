using System;
using System.Collections.Generic;
using CodeModels.Models;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator.Models.Primitives.Expression.AnonymousFunction;

public record AnonymousMethodExpression(Modifier Modifier, bool IsAsync,
    bool IsDelegate, PropertyCollection Parameters,
    IType Type, Block? Body, IExpression? ExpressionBody)
    : AnonymousFunctionExpression<AnonymousMethodExpressionSyntax>(Modifier, Type, Body, ExpressionBody)
{
    public override IEnumerable<ICodeModel> Children()
    {
        foreach (var parameter in Parameters.Properties) yield return parameter;
        if (Body is not null) yield return Body;
        if (ExpressionBody is not null) yield return ExpressionBody;
        foreach (var child in base.Children()) yield return child;
    }

    public override AnonymousMethodExpressionSyntax Syntax()
            => SyntaxFactory.AnonymousMethodExpression(
                IsAsync ? SyntaxFactory.Token(SyntaxKind.AsyncKeyword) : default,
                IsDelegate ? SyntaxFactory.Token(SyntaxKind.DelegateKeyword) : default,
                Parameters.ToParameters(),
                Body?.Syntax(),
                ExpressionBody?.Syntax());


    public override IExpression Evaluate(IProgramModelExecutionContext context)
    {
        return this;
    }

    public override object? EvaluatePlain(IProgramModelExecutionContext context)
    {
        return null;
    }
}