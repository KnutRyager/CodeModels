using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Generation.SyntaxFactoryCustom;
using static CodeModels.Factory.CodeModelFactory;
using CodeModels.Execution.Context;

namespace CodeModels.Models;


public record FinallyClause(IStatement Statement) : CodeModel<FinallyClauseSyntax>
{
    public override FinallyClauseSyntax Syntax() => FinallyClauseCustom(Block(Statement).Syntax());
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Statement;
    }

    public void Evaluate(ICodeModelExecutionContext context)
    {
        context.EnterScope();
        Statement.Evaluate(context);
        context.ExitScope();
    }
}
