using System;
using System.Collections.Generic;
using CodeModels.Execution.Context;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models;

public record ThisExpression(IType Type) : Expression<ThisExpressionSyntax>(Type)
{
    public override ThisExpressionSyntax Syntax() => ThisExpression();

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
    }

    public override object? EvaluatePlain(ICodeModelExecutionContext context) => context.This().EvaluatePlain(context);
    public override IExpression Evaluate(ICodeModelExecutionContext context) => context.This().Evaluate(context);
}