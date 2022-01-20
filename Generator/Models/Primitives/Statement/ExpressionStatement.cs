using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models;

public record ExpressionStatement(IExpression Expression) : AbstractStatement<ExpressionStatementSyntax>
{
    public override ExpressionStatementSyntax Syntax() => ExpressionStatement(Expression.Syntax());

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Expression;
    }
}
