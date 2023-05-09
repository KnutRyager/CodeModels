﻿using System.Collections.Generic;
using System.Linq;
using CodeModels.Execution.Context;
using Common.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models;

public record Block(List<IStatement> Statements) : AbstractStatement<BlockSyntax>
{
    public override BlockSyntax Syntax() => Block(Statements.Select(x => x.Syntax()));
    public Block Add(IStatement statement) => this with { Statements = CollectionUtil.Add(Statements, statement) };
    public override bool EndsInBreak() => Statements.LastOrDefault() is BreakStatement;
    public override IEnumerable<ICodeModel> Children() => Statements;

    public override void Evaluate(ICodeModelExecutionContext context)
    {
        context.EnterScope();
        foreach (var statement in Statements) statement.Evaluate(context);
        context.ExitScope();
    }

    public override string ToString() => $"{{{string.Join(";", Statements)}}}";
}
