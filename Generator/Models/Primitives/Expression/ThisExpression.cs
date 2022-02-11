using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models;

public record ThisExpression(IType Type) : Expression<ThisExpressionSyntax>(Type)
{
    public override ThisExpressionSyntax Syntax() => ThisExpression();

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
    }

    public override IExpression Evaluate(IProgramModelExecutionContext context) => context.This();
}