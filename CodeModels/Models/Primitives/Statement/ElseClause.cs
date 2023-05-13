using System.Collections.Generic;
using CodeModels.Execution.Context;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public record ElseClause(IStatement Statement) : CodeModel<ElseClauseSyntax>
{
    public override ElseClauseSyntax Syntax() => SyntaxFactory.ElseClause(Statement.Syntax());

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Statement;
    }

    public void Evaluate(ICodeModelExecutionContext context)
    {
        try
        {
            context.EnterScope();
            Statement.Evaluate(context);
        }
        finally
        {
            context.ExitScope();
        }
    }

    public override string ToString() => $"else{{{Statement}}}";
}