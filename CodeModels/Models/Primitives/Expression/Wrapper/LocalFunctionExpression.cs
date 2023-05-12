using System;
using CodeModels.Execution.Context;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Reference;
using CodeModels.Models.Primitives.Member;

namespace CodeModels.Models;

/// <summary>
/// Custom Expression wrapper for LocalFunctionStatement.
/// </summary>
public record LocalFunctionExpression(LocalFunctionStatement Function)
    : StatementExpression(Function)
{
    public override IExpression Evaluate(ICodeModelExecutionContext context)
    {
        var name = Identifier().ToString();
        context.DefineVariable(name);
        context.SetValue(name, this);
        return this;
    }

    public override IdentifierExpression Identifier()
        => new(Function.Identifier, Get_Type());

    public override object? EvaluatePlain(ICodeModelExecutionContext context)
    {
        if (Function.Parameters.Properties.Count > 0)
        {
            throw new NotImplementedException();
        }
        else if (Function.Body is Block block)
        {
            //block.Evaluate(context);
            context.EnterScope();
            foreach (var statement in block.Statements) statement.Evaluate(context);
            var result = context.PreviousExpression.EvaluatePlain(context);
            //var result = context.PreviousExpression == this ? Function.EvaluatePlain(context) : context.PreviousExpression.EvaluatePlain(context);
            context.ExitScope();
            return result;
        }
        else if (Function.ExpressionBody is IExpression expression)
        {
            return expression.EvaluatePlain(context);
        }

        throw new NotImplementedException();
    }

    public Method ToMethod() => Function.ToMethod();
}