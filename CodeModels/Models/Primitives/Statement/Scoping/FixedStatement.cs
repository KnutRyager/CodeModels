﻿using System.Collections.Generic;
using CodeModels.Execution.Context;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models;

public record FixedStatement(VariableDeclarations VariableDeclarations, IStatement Statement)
    : AbstractStatement<FixedStatementSyntax>
{
    public static FixedStatement Create(VariableDeclarations variableDeclarations, IStatement statement)
        => new(variableDeclarations, statement);

    public override FixedStatementSyntax Syntax() => FixedStatement(VariableDeclarations.Syntax(), Statement.Syntax());

    public override IEnumerable<ICodeModel> Children()
    {
        yield return VariableDeclarations;
        yield return Statement;
    }

    public override void Evaluate(ICodeModelExecutionContext context)
    {
        throw new System.NotImplementedException();
    }
}
