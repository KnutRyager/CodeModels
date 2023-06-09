﻿using System;
using System.Collections.Generic;
using System.Linq;
using CodeModels.Execution.Context;
using CodeModels.Execution.Scope;
using CodeModels.Models.Interfaces;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Member;
using CodeModels.Utils;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;
using static CodeModels.Generation.SyntaxFactoryCustom;

namespace CodeModels.Models.Primitives.Expression.Invocation;

public record InvocationExpression(Method Method, IExpression? Caller, List<IExpression> Arguments, List<ICodeModelExecutionScope> Scopes)
    : AnyArgExpression<InvocationExpressionSyntax>(new IExpression[] { Caller ?? NullValue }.Concat(Arguments).ToList(), Method.ReturnType, OperationType.Invocation),
    IInvocation
{
    public static InvocationExpression Create(Method method, IExpression? caller, IEnumerable<IExpression>? arguments = null, IEnumerable<ICodeModelExecutionScope>? scopes = null)
        => new(method, caller, List(arguments), List(scopes));

    public override InvocationExpressionSyntax Syntax() => InvocationExpressionCustom(
        SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, Caller?.Syntax() ?? Method.Owner!.TypeSyntax(), SyntaxFactory.IdentifierName(Method.Name)), Arguments.Select(x => x.Syntax()));

    public override IExpression Evaluate(ICodeModelExecutionContext context)
        => Literal(EvaluatePlain(context));

    public override object? EvaluatePlain(ICodeModelExecutionContext context)
    {
        try
        {
            context.EnterScopes(Scopes);
            if (Caller is IInstantiatedObject instance)
            {
                instance.EnterScopes(context);
            }
            else
            {
                context.EnterScope();
            }

            if (Method.Body is null && Method.ExpressionBody is null)
            {
                throw new NotImplementedException();
            }
            for (var i = 0; i < Method.Parameters.Parameters.Count; i++)
            {
                var argument = Arguments.Count > i ? Arguments[i] : VoidValue;
                var parameter = Method.Parameters.Parameters[i];
                context.DefineVariable(parameter.Name);
                context.SetValue(parameter.Name, ExpressionUtils.IsVoid(argument) ? parameter.Expression ?? VoidValue : argument);
            }

            if (Method.Body is Block block)
            {
                //block.Evaluate(context);
                foreach (var statement in block.Statements) statement.Evaluate(context);
                var result = context.PreviousExpression.EvaluatePlain(context);
                //var result = context.PreviousExpression == this ? Function.EvaluatePlain(context) : context.PreviousExpression.EvaluatePlain(context);

                return result;
            }
            else
            {
                var result = Method.ExpressionBody!.EvaluatePlain(context);
                return result;
            }
        }
        finally
        {
            if (Caller is IInstantiatedObject instance)
            {
                instance.ExitScopes(context);
            }
            else
            {
                context.ExitScope();
            }
            context.ExitScopes(Scopes);
        }
    }
}