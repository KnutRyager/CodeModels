using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public record ConstantPattern(IExpression Expression)
    : Pattern<ConstantPatternSyntax>
{
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Expression;
    }

    public override ConstantPatternSyntax Syntax()
        => SyntaxFactory.ConstantPattern(Expression.Syntax());
}
