using System.Collections.Generic;
using CodeModels.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator.Models.Primitives.Expression.AnonymousFunction;

public abstract record AnonymousFunctionExpression<T>(Modifier Modifier,
     IType Type, Block? Body, IExpression? ExpressionBody)
    : Expression<T>(Type), IAnonymousFunctionExpression<T>
    where T : AnonymousFunctionExpressionSyntax
{
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
        if (Body is not null) yield return Body;
        if (ExpressionBody is not null) yield return ExpressionBody;
    }

    AnonymousFunctionExpressionSyntax IAnonymousFunctionExpression.Syntax()
        => Syntax();
}