using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static CodeAnalyzation.Models.CodeModelFactory;

namespace CodeAnalyzation.Models;


public record FinallyClause(IStatement Statement) : CodeModel<FinallyClauseSyntax>
{
    public override FinallyClauseSyntax Syntax() => FinallyClauseCustom(Block(Statement).Syntax());
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Statement;
    }

    public void Evaluate(IProgramModelExecutionContext context)
    {
        context.EnterScope();
        Statement.Evaluate(context);
        context.ExitScope();
    }
}
