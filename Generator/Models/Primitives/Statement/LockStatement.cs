using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models;

public record LockStatement(IExpression Expression, IStatement Statement) : AbstractStatement<LockStatementSyntax>
{
    public override LockStatementSyntax Syntax() => LockStatement(Expression.Syntax(), Statement.Syntax());

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Expression;
        yield return Statement;
    }

    public override void Evaluate(IProgramModelExecutionContext context)
    {
        throw new System.NotImplementedException();
    }
}
