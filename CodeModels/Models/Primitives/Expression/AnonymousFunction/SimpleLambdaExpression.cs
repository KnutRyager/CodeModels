﻿using System;
using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Execution.ControlFlow;
using CodeModels.Execution.Scope;
using CodeModels.Factory;
using CodeModels.Models;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Member;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Generator.Models.Primitives.Expression.AnonymousFunction;

public record SimpleLambdaExpression(Modifier Modifier,
    bool IsAsync,
    Parameter Parameter,
    IType Type,
    Block? Body,
    IExpression? ExpressionBody)
    : LambdaExpression<SimpleLambdaExpressionSyntax>
    (Modifier, IsAsync, Parameter.ToParameterList(), Type, Body, ExpressionBody), ILambdaExpression
{
    public static SimpleLambdaExpression Create(IToParameterConvertible parameter,
        IType type,
        IStatementOrExpression? body = null,
        bool? isAsync = default,
        MethodBodyPreference? bodyPreference = default,
        Modifier? modifier = default)
        => new(modifier ?? Modifier.None,
            isAsync ?? false,
            parameter.ToParameter(),
            type,
            MethodUtils.GetBodyFromPreference(body, bodyPreference ?? MethodBodyPreference.Expression),
            MethodUtils.GetExpressionBodyFromPreference(body, bodyPreference ?? MethodBodyPreference.Expression));

    public override IEnumerable<ICodeModel> Children()
    {
        foreach (var child in base.Children()) yield return child;
    }

    public override SimpleLambdaExpressionSyntax Syntax()
        => SyntaxFactory.SimpleLambdaExpression(
                IsAsync ? TokenList(Token(SyntaxKind.AsyncKeyword)) : default,
                Parameter.TypelessSyntax(),
                Body?.Syntax(),
                ExpressionBody?.Syntax());

    public override IExpression Evaluate(ICodeModelExecutionContext context)
    {
        return this;
    }

    // https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.dynamicmethod?view=net-7.0
    public override object? EvaluatePlain(ICodeModelExecutionContext context)
    {
        if (Type.Name is "Action")
        {
            System.Linq.Expressions.Expression<Action<object>> action = x =>
            InnerEvaluate(x, context, context.CaptureScope());
            return action.Compile();
        }
        System.Linq.Expressions.Expression<Func<object, object?>> expression = x =>
        InnerEvaluate(x, context, context.CaptureScope());
        return expression.Compile();
    }

    private dynamic? InnerEvaluate(dynamic argument, ICodeModelExecutionContext context, ICodeModelExecutionScope scope)
    {
        context.EnterScope(scope);
        context.EnterScope();
        context.DefineVariable(Parameter.Name);
        context.SetValue(Parameter.Name, CodeModelFactory.Literal(argument));
        if (ExpressionBody is not null)
        {
            var result = ExpressionBody.EvaluatePlain(context);
            context.ExitScope();
            context.ExitScope();
            return result;
        }
        try
        {
            Body?.Evaluate(context);
            context.ExitScope();
            context.ExitScope();
        }
        catch (ReturnException e)
        {
            context.ExitScope();
            context.ExitScope();
            return e.Value;
        }
        return null;
    }

    LambdaExpressionSyntax ILambdaExpression.Syntax()
        => Syntax();
}
