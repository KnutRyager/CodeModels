using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public record PropertyExpression(Property Property, IExpression? Instance = null) : Expression<ExpressionSyntax>(Property.Type)
{
    public override ExpressionSyntax Syntax() => Property?.AccessSyntax(Instance) ?? ((IExpression)this).Syntax();

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
        if (Instance is not null) yield return Instance;
    }
}
