using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models;

public record AwaitExpression(IExpression Expression)  : Expression<AwaitExpressionSyntax>(Expression.Get_Type())
{
    public override AwaitExpressionSyntax Syntax() => AwaitExpression(Expression.Syntax());
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Expression;
    }

    public override IExpression Evaluate(IProgramModelExecutionContext context)
    {
        throw new System.NotImplementedException();
    }
}
