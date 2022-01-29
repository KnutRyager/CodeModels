using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static CodeAnalyzation.Models.CodeModelFactory;

namespace CodeAnalyzation.Models;

public record ForEachStatement(IType? Type, string Identifier, IExpression Expression, IStatement Statement)
    : AbstractStatement<ForEachStatementSyntax>
{
    public ForEachStatement(string Identifier, IExpression Expression, IStatement Statement)
        : this(null, Identifier, Expression, Statement) { }

    public override ForEachStatementSyntax Syntax() => ForEachStatementCustom((Type ?? TypeShorthands.VarType).Syntax(),
        SyntaxFactory.Identifier(Identifier),
        Expression.Syntax(),
        Statement.Syntax());

    public override IEnumerable<ICodeModel> Children()
    {
        if (Type is not null) yield return Type;
        yield return Expression;
        yield return Statement;
    }

    public override void Evaluate(IProgramModelExecutionContext context)
    {
        context.EnterScope();
        context.DefineVariable(Identifier);
        var iterator = Expression.Evaluate(context).LiteralValue as System.Collections.IEnumerable;
        if (iterator is null)
        {
            context.Throw(new NullReferenceException("null iterator"));
            return;
        }
        foreach (var value in iterator)
        {
            context.SetValue(Identifier, Literal(value));
            Statement.Evaluate(context);
            if (context.HandleReturn() || context.HandleThrow()) return;
            if (context.HandleBreak()) break;
            if (context.HandleContinue()) continue;
        }
        context.ExitScope();
    }
}
