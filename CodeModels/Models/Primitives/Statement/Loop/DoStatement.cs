﻿using System;
using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Execution.ControlFlow;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Generation.SyntaxFactoryCustom;

namespace CodeModels.Models;

public record DoStatement(IStatement Statement, IExpression Condition) : AbstractStatement<DoStatementSyntax>
{
    public override DoStatementSyntax Syntax() => DoStatementCustom(Statement.Syntax(), Condition.Syntax());
    public override IEnumerable<ICodeModel> Children() => Array.Empty<ICodeModel>();

    public override void Evaluate(ICodeModelExecutionContext context)
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
