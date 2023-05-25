using System.Collections.Generic;
using CodeModels.AbstractCodeModels.Collection;
using CodeModels.Execution.Context;
using CodeModels.Models;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator.Models.Primitives.Expression.AnonymousFunction;

public record AnonymousMethodExpression(Modifier Modifier, bool IsAsync,
    bool IsDelegate, NamedValueCollection Parameters,
    IType Type, Block? Body, IExpression? ExpressionBody)
    : AnonymousFunctionExpression<AnonymousMethodExpressionSyntax>(Modifier, Type, Body, ExpressionBody)
{
    public static AnonymousMethodExpression Create(Modifier modifier, bool isAsync,
    bool isDelegate, NamedValueCollection parameters,
    IType type, Block? body = null, IExpression? expressionBody = null)
        => new(modifier, isAsync, isDelegate, parameters, type, body, expressionBody);

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


    public override IExpression Evaluate(ICodeModelExecutionContext context)
    {
        return this;
    }

    public override object? EvaluatePlain(ICodeModelExecutionContext context)
    {
        return null;
    }
}