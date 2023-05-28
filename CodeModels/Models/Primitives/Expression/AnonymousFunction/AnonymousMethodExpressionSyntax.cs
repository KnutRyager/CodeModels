using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Models;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Member;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;

namespace Generator.Models.Primitives.Expression.AnonymousFunction;

public record AnonymousMethodExpression(Modifier Modifier, bool IsAsync,
    bool IsDelegate, ParameterList Parameters,
    IType Type, Block? Body, IExpression? ExpressionBody)
    : AnonymousFunctionExpression<AnonymousMethodExpressionSyntax>(Modifier, Type, Body, ExpressionBody)
{
    public static AnonymousMethodExpression Create(Modifier modifier, bool isAsync,
    bool isDelegate, IToParameterListConvertible? parameters,
    IType type, Block? body = null, IExpression? expressionBody = null)
        => new(modifier, isAsync, isDelegate, parameters?.ToParameterList() ?? ParamList(), type, body, expressionBody);

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Parameters;
        if (Body is not null) yield return Body;
        if (ExpressionBody is not null) yield return ExpressionBody;
        foreach (var child in base.Children()) yield return child;
    }

    public override AnonymousMethodExpressionSyntax Syntax()
            => SyntaxFactory.AnonymousMethodExpression(
                IsAsync ? SyntaxFactory.Token(SyntaxKind.AsyncKeyword) : default,
                IsDelegate ? SyntaxFactory.Token(SyntaxKind.DelegateKeyword) : default,
                Parameters.Syntax(),
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