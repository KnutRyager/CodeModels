using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Generation.SyntaxFactoryCustom;
using static CodeModels.Factory.CodeModelFactory;
using CodeModels.Execution.Context;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis.CSharp;
using CodeModels.Execution;

namespace CodeModels.Models;

public record IfStatement(IExpression Condition, IStatement Statement, IStatement? Else = null) : AbstractStatement<IfStatementSyntax>
{
    public override IfStatementSyntax Syntax() => SyntaxFactory.IfStatement(
     SyntaxFactory.List<AttributeListSyntax>(),
     Condition.Syntax(),
     Block(Statement).Syntax(),
     Else is null ? null : ElseClauseCustom((Else is IfStatement or MultiIfStatement ? Else : Block(Else)).Syntax()));

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Condition;
        yield return Statement;
        if (Else is not null) yield return Else;
    }

    public override void Evaluate(ICodeModelExecutionContext context)
    {
        var condition = Condition.Evaluate(context).LiteralValue();
        if (condition is bool b ? b : throw new CodeModelExecutionException($"Not a conditional: {Condition}"))
        {
            context.EnterScope();
            Statement.Evaluate(context);
            context.ExitScope();
        }
        else if (Else is not null)
        {
            context.EnterScope();
            Else.Evaluate(context);
            context.ExitScope();
        }
    }

    public override string ToString() => $"if({Condition}){{{Statement}}}{(Else is null ? "" : $" else{{{Else}}}")}";
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

    public override void Evaluate(ICodeModelExecutionContext context)
    {
        var ifWasExecuted = false;
        foreach (var statement in IfStatements)
        {
            var ifCondition = (bool)(statement.Condition.Evaluate(context).LiteralValue() ?? false);
            ifWasExecuted |= ifCondition;
            if (ifCondition)
            {
                context.EnterScope();
                statement.Evaluate(context);
                context.ExitScope();
            }
            if (ifWasExecuted) break;
        }
        if (!ifWasExecuted && Else is not null)
        {
            context.EnterScope();
            Else.Evaluate(context);
            context.ExitScope();
        }
    }

    public override string ToString() => $"{(
        IfStatements.Count > 0 ?
            $"ifs({string.Join("", IfStatements.Select(x => x.ToString()))})"
            : ""
        )}{(Else is null ? "" : $" else {{{Else}}}")}";
}
