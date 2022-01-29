using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;

namespace CodeAnalyzation.Models;

public record WhileStatement(IExpression Condition, IStatement Statement) : AbstractStatement<WhileStatementSyntax>
{
    public override WhileStatementSyntax Syntax() => WhileStatementCustom(Condition.Syntax(), Statement.Syntax());
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Condition;
        yield return Statement;
    }

    public override void Evaluate(IProgramModelExecutionContext context)
    {
        context.EnterScope();
        while ((bool)Condition.Evaluate(context).LiteralValue)
        {
            Statement.Evaluate(context);
            if (context.HandleReturn() || context.HandleThrow()) return;
            if (context.HandleBreak()) break;
            if (context.HandleContinue()) continue;
        }
        context.ExitScope();
    }
}
