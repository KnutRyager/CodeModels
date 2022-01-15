﻿using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;

namespace CodeAnalyzation.Models
{
    public record ReturnStatement(IExpression Expression) : AbstractStatement<ReturnStatementSyntax>
    {
        public override ReturnStatementSyntax Syntax() => ReturnCustom(Expression.Syntax());

        public override IEnumerable<ICodeModel> Children()
        {
            yield return Expression;
        }
    }
}