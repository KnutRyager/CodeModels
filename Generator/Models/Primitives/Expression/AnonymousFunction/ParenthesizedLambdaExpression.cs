﻿using System;
using System.Collections.Generic;
using CodeAnalyzation.Models;
using CodeAnalyzation.Models.Execution.ControlFlow;
using Common.Util;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Generator.Models.Primitives.Expression.AnonymousFunction;

public record ParenthesizedLambdaExpression(Modifier Modifier,
    bool IsAsync,
    PropertyCollection Parameters,
    IType Type,
    Block? Body,
    IExpression? ExpressionBody)
    : LambdaExpression<ParenthesizedLambdaExpressionSyntax>
    (Modifier, IsAsync, Parameters, Type, Body, ExpressionBody), ILambdaExpression
{
    public override IEnumerable<ICodeModel> Children()
    {
        foreach (var child in base.Children()) yield return child;
    }

    public override ParenthesizedLambdaExpressionSyntax Syntax()
        => SyntaxFactory.ParenthesizedLambdaExpression(
                IsAsync ? Token(SyntaxKind.AsyncKeyword) : default,
                Parameters.ToParameters(),
                Token(SyntaxKind.ArrowExpressionClause),
                Body?.Syntax(),
                ExpressionBody?.Syntax());

    public override IExpression Evaluate(IProgramModelExecutionContext context)
    {
        return this;
    }

    public override object? EvaluatePlain(IProgramModelExecutionContext context)
    {
        if (Type.Name is "Action")
        {
            return Parameters.Properties.Count switch
            {
                0 => FunctionalUtil.ActionExpression(() => InnerEvaluate(Array.Empty<object>(), context)).Compile(),
                1 => FunctionalUtil.ActionExpression<object>((x) => InnerEvaluate(new object[] { x }, context)).Compile(),
                2 => FunctionalUtil.ActionExpression<object, object>((x1, x2) => InnerEvaluate(new object[] { x1, x2 }, context)).Compile(),
                3 => FunctionalUtil.ActionExpression<object, object, object>((x1, x2, x3) => InnerEvaluate(new object[] { x1, x2, x3 }, context)).Compile(),
                4 => FunctionalUtil.ActionExpression<object, object, object, object>((x1, x2, x3, x4) => InnerEvaluate(new object[] { x1, x2, x3, x4 }, context)).Compile(),
                5 => FunctionalUtil.ActionExpression<object, object, object, object, object>((x1, x2, x3, x4, x5) => InnerEvaluate(new object[] { x1, x2, x3, x4, x5 }, context)).Compile(),
                6 => FunctionalUtil.ActionExpression<object, object, object, object, object, object>((x1, x2, x3, x4, x5, x6) => InnerEvaluate(new object[] { x1, x2, x3, x4, x5, x6 }, context)).Compile(),
                _ => throw new NotImplementedException()
            };
        }
        return Parameters.Properties.Count switch
        {
            0 => FunctionalUtil.LambdaExpression<object?>(() => InnerEvaluate(Array.Empty<object>(), context)).Compile(),
            1 => FunctionalUtil.LambdaExpression<object, object?>((x) => InnerEvaluate(new object[] { x }, context)).Compile(),
            2 => FunctionalUtil.LambdaExpression<object, object, object?>((x1, x2) => InnerEvaluate(new object[] { x1, x2 }, context)).Compile(),
            3 => FunctionalUtil.LambdaExpression<object, object, object, object?>((x1, x2, x3) => InnerEvaluate(new object[] { x1, x2, x3 }, context)).Compile(),
            4 => FunctionalUtil.LambdaExpression<object, object, object, object, object?>((x1, x2, x3, x4) => InnerEvaluate(new object[] { x1, x2, x3, x4 }, context)).Compile(),
            5 => FunctionalUtil.LambdaExpression<object, object, object, object, object, object?>((x1, x2, x3, x4, x5) => InnerEvaluate(new object[] { x1, x2, x3, x4, x5 }, context)).Compile(),
            6 => FunctionalUtil.LambdaExpression<object, object, object, object, object, object, object?>((x1, x2, x3, x4, x5, x6) => InnerEvaluate(new object[] { x1, x2, x3, x4, x5, x6 }, context)).Compile(),
            _ => throw new NotImplementedException()
        };
    }

    private dynamic? InnerEvaluate(dynamic[] arguments, IProgramModelExecutionContext context)
    {
        context.EnterScope();
        for (var i = 0; i < Parameters.Properties.Count; i++)
        {
            var parameter = Parameters.Properties[i];
            var argument = arguments[i];
            context.DefineVariable(parameter.Name);
            context.SetValue(parameter.Name, new LiteralExpression(argument));
        }
        if (ExpressionBody is not null)
        {
            var result = ExpressionBody.EvaluatePlain(context);
            context.ExitScope();
            return result;
        }
        try
        {
            Body?.Evaluate(context);
            context.ExitScope();
        }
        catch (ReturnException e)
        {
            context.ExitScope();
            return e.Value;
        }
        return null;
    }

    LambdaExpressionSyntax ILambdaExpression.Syntax()
        => Syntax();
}