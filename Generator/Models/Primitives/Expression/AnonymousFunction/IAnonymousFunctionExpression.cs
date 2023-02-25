using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public interface IAnonymousFunctionExpression : IExpression
{
    new AnonymousFunctionExpressionSyntax Syntax();
}

public interface IAnonymousFunctionExpression<T> : IAnonymousFunctionExpression, IExpression<T>
    where T : AnonymousFunctionExpressionSyntax
{
    new T Syntax();
}