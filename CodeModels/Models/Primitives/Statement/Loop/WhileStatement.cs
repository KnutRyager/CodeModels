using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Execution.ControlFlow;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Generation.SyntaxFactoryCustom;

namespace CodeModels.Models;

public record WhileStatement(IExpression Condition, IStatement Statement) : AbstractStatement<WhileStatementSyntax>
{
    public override WhileStatementSyntax Syntax() => WhileStatementCustom(Condition.Syntax(), Statement.Syntax());
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Condition;
        yield return Statement;
    }

    public override void Evaluate(ICodeModelExecutionContext context)
    {
        context.EnterScope();
        while (((bool?)Condition.Evaluate(context).LiteralValue()) ?? false)
        {
            try
            {
                Statement.Evaluate(context);
            }
            catch (BreakException)
            {
                break;
            }
            catch (ContinueException)
            {
                continue;
            }
        }
        context.ExitScope();
    }
}
