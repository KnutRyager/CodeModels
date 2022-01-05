using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static CodeAnalyzation.Models.CodeModelFactory;

namespace CodeAnalyzation.Models
{

    public record ThrowStatement(IExpression Expression) : AbstractStatement<ThrowStatementSyntax>
    {
        public override ThrowStatementSyntax Syntax() => ThrowStatementCustom(Expression.Syntax());
    }

    public record ThrowExpression(IExpression Expression) : Expression<ThrowExpressionSyntax>(Expression.Type)
    {
        public override ThrowExpressionSyntax Syntax() => ThrowExpressionCustom(Expression.Syntax());
    }
}