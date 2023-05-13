using System;
using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Execution.ControlFlow;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public record CatchFilterClause(IExpression Expression) : CodeModel<CatchFilterClauseSyntax>
{
    public static CatchFilterClause Create(IExpression expression) => new(expression);

    public override CatchFilterClauseSyntax Syntax()
        => SyntaxFactory.CatchFilterClause(Expression.Syntax());

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Expression;
    }

    public void Evaluate(ThrowException exception, ICodeModelExecutionContext context)
        => throw new NotImplementedException();
}
