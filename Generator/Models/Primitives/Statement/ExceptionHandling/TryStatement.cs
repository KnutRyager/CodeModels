using System;
using System.Collections.Generic;
using System.Linq;
using CodeAnalyzation.Models.Execution.Controlflow;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static CodeAnalyzation.Models.CodeModelFactory;

namespace CodeAnalyzation.Models;

public record TryStatement(IStatement Statement, List<CatchClause> CatchClauses, FinallyClause? Finally = null) : AbstractStatement<TryStatementSyntax>
{
    public override TryStatementSyntax Syntax() => TryStatementCustom(
         Block(Statement).Syntax(), CatchClauses.Select(x => x.Syntax()), Finally?.Syntax());

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Statement;
        foreach (var catchClause in CatchClauses)
        {
            yield return catchClause;
        }
        if (Finally is not null) yield return Finally;
    }

    public override void Evaluate(IProgramModelExecutionContext context)
    {
        try
        {
            context.EnterScope();
            Statement.Evaluate(context);
            context.ExitScope();
        }
        catch (ThrowException e)
        {
            context.ExitScope();
            foreach (var catchClause in CatchClauses)
            {
                catchClause.Evaluate(e, context);
            }
        }
        finally
        {
            Finally?.Evaluate(context);
        }
    }
}