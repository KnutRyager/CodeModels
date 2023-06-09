﻿using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models;

public record LockStatement(IExpression Expression, IStatement Statement)
    : AbstractStatement<LockStatementSyntax>
{
    public static LockStatement Create(IExpression expression, IStatement statement)
        => new(expression, statement);

    public override LockStatementSyntax Syntax() => LockStatement(Expression.Syntax(), Statement.Syntax());

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Expression;
        yield return Statement;
    }

    public override void Evaluate(ICodeModelExecutionContext context)
    {
        throw new System.NotImplementedException();
    }
}
