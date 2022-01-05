﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;

namespace CodeAnalyzation.Models
{
    public record DoStatement(IStatement Statement, IExpression Condition) : AbstractStatement<DoStatementSyntax>
    {
        public override DoStatementSyntax Syntax() => DoStatementCustom(Statement.Syntax(), Condition.Syntax());
    }
}