using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Execution.ControlFlow;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public record ReturnStatement(IExpression? Expression) : AbstractStatement<ReturnStatementSyntax>
{
    public static ReturnStatement Create(IExpression? expression = null) => new(expression);

    public override ReturnStatementSyntax Syntax() => SyntaxFactory.ReturnStatement(Expression?.Syntax());

    public override IEnumerable<ICodeModel> Children()
    {
        if (Expression is not null) yield return Expression;
    }

    public override void Evaluate(ICodeModelExecutionContext context)
        => throw new ReturnException(Expression?.EvaluatePlain(context));
}
