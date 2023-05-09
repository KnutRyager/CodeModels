using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public interface IAnonymousFunctionExpression : IExpression
{
    new AnonymousFunctionExpressionSyntax Syntax();
}

public interface IAnonymousFunctionExpression<T> : IAnonymousFunctionExpression, IExpression<T>
    where T : AnonymousFunctionExpressionSyntax
{
    new T Syntax();
}