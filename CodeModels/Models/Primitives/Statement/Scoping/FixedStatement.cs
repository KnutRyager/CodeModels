using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models;

public record FixedStatement(VariableDeclarations VariableDeclarations, IStatement Statement) : AbstractStatement<FixedStatementSyntax>
{
    public override FixedStatementSyntax Syntax() => FixedStatement(VariableDeclarations.Syntax(), Statement.Syntax());

    public override IEnumerable<ICodeModel> Children()
    {
        yield return VariableDeclarations;
        yield return Statement;
    }

    public override void Evaluate(IProgramModelExecutionContext context)
    {
        throw new System.NotImplementedException();
    }
}
