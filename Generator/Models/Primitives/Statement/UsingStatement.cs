using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models;

public record UsingStatement(IStatement Statement) : AbstractStatement<UsingStatementSyntax>
{
    public override UsingStatementSyntax Syntax() => UsingStatement(Statement.Syntax());

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Statement;
    }

    public override void Evaluate(IProgramModelExecutionContext context)
    {
        throw new System.NotImplementedException();
    }
}
