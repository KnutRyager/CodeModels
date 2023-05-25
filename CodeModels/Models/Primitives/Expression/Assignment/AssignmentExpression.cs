using System;
using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Execution.Scope;
using CodeModels.Factory;
using CodeModels.Models.Interfaces;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models;

public record AssignmentExpression(IExpression Left, IExpression Right, AssignmentType Kind)
    : Expression<AssignmentExpressionSyntax>(Left.Get_Type())
{
    public static AssignmentExpression Create(IExpression left, IExpression right, AssignmentType kind)
        => new(left, right, kind);

    public override AssignmentExpressionSyntax Syntax() => AssignmentExpression(Kind.Syntax(), Left.Syntax(), Right.Syntax());
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Left;
        yield return Right;
    }

    public override IExpression Evaluate(ICodeModelExecutionContext context)
    {
        var scopes = Left is IScopeHolder scopeHolder ? scopeHolder.GetScopes(context) : Array.Empty<ICodeModelExecutionScope>();
        //if (Left is IInvokable invokable)
        //{
        //    invokable.Invoke(Right, context, scopes).Evaluate();
        //    return Right;
        //}else
        var rightEvaluated = Right.Evaluate(context);
        var result = Kind is AssignmentType.Simple ? rightEvaluated : CodeModelFactory.BinaryExpression(Left, Kind.GetOperationType(), rightEvaluated).Evaluate(context);

        if (Left is IAssignable assignable)
        {
            assignable.Assign(result, context, scopes);
        }
        else
        {
            context.SetValue(Left, result);
        }
        return result;
    }
}