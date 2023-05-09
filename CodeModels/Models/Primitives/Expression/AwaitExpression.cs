﻿using System.Collections.Generic;
using System.Threading.Tasks;
using CodeModels.Execution.Context;
using Common.Reflection;
using Common.Util;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Common.Util.TaskExtensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models;

public record AwaitExpression(IExpression Expression) : Expression<AwaitExpressionSyntax>(Expression.Get_Type())
{
    public override AwaitExpressionSyntax Syntax() => AwaitExpression(Expression.Syntax());
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Expression;
    }

    public override IExpression Evaluate(ICodeModelExecutionContext context)
    {
        var value = Expression.Evaluate(context);
        if (value.LiteralValue() is Task task)
        {
            if (task.GetType().IsGenericType)
            {
                return new LiteralExpression(ReflectionUtil.ConvertTaskResult(value.LiteralValue() as Task));
            }
            task.WaitAndUnwrapException();
        }
        return value;
    }
}
