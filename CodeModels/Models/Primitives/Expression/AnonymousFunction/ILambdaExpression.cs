using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public interface ILambdaExpression : IAnonymousFunctionExpression
{
    new LambdaExpressionSyntax Syntax();
}

public interface ILambdaExpression<T> : ILambdaExpression, IAnonymousFunctionExpression<T>
    where T : LambdaExpressionSyntax
{
    new T Syntax();
}