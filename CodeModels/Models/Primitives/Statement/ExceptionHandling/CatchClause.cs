﻿using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Generation.SyntaxFactoryCustom;
using static CodeModels.Factory.CodeModelFactory;
using CodeModels.Execution.ControlFlow;
using CodeModels.Execution.Context;

namespace CodeModels.Models;

public record CatchClause(IType Type, string? Identifier, IStatement Statement) : CodeModel<CatchClauseSyntax>
{
    public override CatchClauseSyntax Syntax() => CatchClauseCustom(
        CatchDeclarationCustom(Type.Syntax(), Identifier), Block(Statement).Syntax());
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
        yield return Statement;
    }

    public void Evaluate(ThrowException exception, ICodeModelExecutionContext context)
    {
        if (!Match(exception, context)) return;
        context.EnterScope();
        if (Identifier is not null)
        {
            context.DefineVariable(Identifier);
            context.SetValue(Identifier, exception.Expression);
        }
        Statement.Evaluate(context);
        context.ExitScope();
    }

    private bool Match(ThrowException exception, ICodeModelExecutionContext context)
        => Type.IsAssignableFrom(exception.Expression.Get_Type(), context);
}
