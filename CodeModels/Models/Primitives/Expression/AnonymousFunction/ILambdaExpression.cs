using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public interface ILambdaExpression : IAnonymousFunctionExpression
{
    new LambdaExpressionSyntax Syntax();
    Block? Body { get; }
    IExpression? ExpressionBody { get; }
}

public interface ILambdaExpression<T> : ILambdaExpression, IAnonymousFunctionExpression<T>
    where T : LambdaExpressionSyntax
{
    new T Syntax();
}