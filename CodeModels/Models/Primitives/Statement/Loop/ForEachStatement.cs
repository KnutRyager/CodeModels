﻿using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Generation.SyntaxFactoryCustom;
using static CodeModels.Factory.CodeModelFactory;
using CodeModels.Execution.ControlFlow;
using CodeModels.Execution.Context;
using CodeModels.Models.Primitives.Expression.Abstract;

namespace CodeModels.Models;

public record ForEachStatement(IType? Type, string Identifier, IExpression Expression, IStatement Statement)
    : AbstractStatement<ForEachStatementSyntax>
{
    public static ForEachStatement Create(IType? type, string identifier, IExpression expression, IStatement statement)
        => new(type, identifier, expression, statement);

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

    public override void Evaluate(ICodeModelExecutionContext context)
    {
        context.EnterScope();
        context.DefineVariable(Identifier);
        if (Expression.Evaluate(context).LiteralValue() is not System.Collections.IEnumerable iterator)
        {
            context.Throw(new NullReferenceException("null iterator"));
            return;
        }
        foreach (var value in iterator)
        {
            try
            {
                context.IncreaseDisableSetPreviousValueLock();
                context.SetValue(Identifier, Literal(value));
            }
            finally
            {
                context.DecreaseDisableSetPreviousValueLock();
            }
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
