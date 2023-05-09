using System;
using System.Collections.Generic;
using CodeAnalyzation.Models.Execution.ControlFlow;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;

namespace CodeAnalyzation.Models;

public record DoStatement(IStatement Statement, IExpression Condition) : AbstractStatement<DoStatementSyntax>
{
    public override DoStatementSyntax Syntax() => DoStatementCustom(Statement.Syntax(), Condition.Syntax());
    public override IEnumerable<ICodeModel> Children() => Array.Empty<ICodeModel>();

    public override void Evaluate(IProgramModelExecutionContext context)
    {
        context.EnterScope();
        do
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
        } while (((bool?)Condition.Evaluate(context).LiteralValue()) ?? false);
        context.ExitScope();
    }
}
