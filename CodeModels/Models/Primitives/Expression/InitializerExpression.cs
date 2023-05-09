using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models;

// TODO: Determine arguments vs initializer
public record InitializerExpression(IType Type, SyntaxKind Kind, PropertyCollection Expressions) : Expression<InitializerExpressionSyntax>(Type)
{
    public override InitializerExpressionSyntax Syntax() => InitializerExpression(Kind, Expressions.SyntaxList());

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
        yield return Expressions;
    }

    public override IExpression Evaluate(IProgramModelExecutionContext context) => Expressions.Evaluate(context);
}
