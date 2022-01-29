using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static CodeAnalyzation.Models.CodeModelFactory;

namespace CodeAnalyzation.Models;

public record IfStatement(IExpression Condition, IStatement Statement, IStatement? Else = null) : AbstractStatement<IfStatementSyntax>
{
    public override IfStatementSyntax Syntax() => IfStatementCustom(
        Condition.Syntax(), Block(Statement).Syntax(), Else is null ? null : ElseClauseCustom(Block(Else).Syntax()));

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Condition;
        yield return Statement;
        if (Else is not null) yield return Else;
    }

    public override void Evaluate(IProgramModelExecutionContext context)
    {
        if ((bool)Condition.Evaluate(context).LiteralValue)
        {
            context.EnterScope();
            Statement.Evaluate(context);
            if (context.HandleReturn() || context.HandleThrow()) return;
            context.ExitScope();
        }
        else if (Else is not null)
        {
            context.EnterScope();
            Else.Evaluate(context);
            if (context.HandleReturn() || context.HandleThrow()) return;
            context.ExitScope();
        }
    }
}

public record MultiIfStatement(List<IfStatement> IfStatements, IStatement? Else) : AbstractStatement<IfStatementSyntax>
{
    public override IfStatementSyntax Syntax()
    {
        var reversed = IfStatements.AsEnumerable().Reverse().ToList();
        var ifs = reversed.First().Syntax();
        if (Else is not null) ifs = ifs.WithElse(ElseClauseCustom(Block(Else).Syntax()));
        for (var i = 1; i < reversed.Count; i++)
        {
            ifs = reversed[i].Syntax().WithElse(ElseClauseCustom(ifs));
        }
        return ifs;
    }

    public override IEnumerable<ICodeModel> Children()
    {
        foreach (var statement in IfStatements) yield return statement;
        if (Else is not null) yield return Else;
    }

    public override void Evaluate(IProgramModelExecutionContext context)
    {
        var ifWasExecuted = false;
        foreach (var statement in IfStatements)
        {
            var ifCondition = (bool)statement.Condition.Evaluate(context).LiteralValue;
            ifWasExecuted |= ifCondition;
            if (ifCondition)
            {
                context.EnterScope();
                statement.Evaluate(context);
                if (context.HandleReturn() || context.HandleThrow()) return;
                context.ExitScope();
            }
            if (ifWasExecuted) break;
        }
        if (!ifWasExecuted && Else is not null)
        {
            context.EnterScope();
            Else.Evaluate(context);
            if (context.HandleReturn() || context.HandleThrow()) return;
            context.ExitScope();
        }
    }
}
