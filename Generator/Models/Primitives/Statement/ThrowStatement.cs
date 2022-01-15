using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;

namespace CodeAnalyzation.Models;

public record ThrowStatement(IExpression Expression) : AbstractStatement<ThrowStatementSyntax>
{
    public override ThrowStatementSyntax Syntax() => ThrowStatementCustom(Expression.Syntax());
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Expression;
    }
}

public record ThrowExpression(IExpression Expression) : Expression<ThrowExpressionSyntax>(Expression.Type)
{
    public override ThrowExpressionSyntax Syntax() => ThrowExpressionCustom(Expression.Syntax());
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Expression;
    }
}
