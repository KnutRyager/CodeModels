using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static CodeAnalyzation.Models.CodeModelFactory;

namespace CodeAnalyzation.Models;

/// <summary>
/// Custom Expression wrapper for LocalFunctionStatement.
/// </summary>
public record LocalFunctionExpression(LocalFunctionStatement Function)
    : StatementExpression(Function)
{
    public override IExpression Evaluate(IProgramModelExecutionContext context)
    {
        var name = GetIdentifier().ToString();
        context.DefineVariable(name);
        context.SetValue(name, this);
        return this;
    }

    public override IdentifierExpression GetIdentifier()
        => new(Function.Identifier, Get_Type());

    public override object? EvaluatePlain(IProgramModelExecutionContext context)
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
}