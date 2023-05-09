using System;
using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

// TODO
public record GotoStatement(IExpression Expression) : AbstractStatement<GotoStatementSyntax>
{
    public override GotoStatementSyntax Syntax() => throw new NotImplementedException();//GotoStatement(SyntaxKind.Block ,Case.Syntax(), Expression.Syntax());

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Expression;
    }

    public override void Evaluate(ICodeModelExecutionContext context)
    {
        throw new NotImplementedException();
    }
}
