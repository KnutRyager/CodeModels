using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Member;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public interface ILambdaExpression : IAnonymousFunctionExpression
{
    ParameterList Parameters { get; }
    new LambdaExpressionSyntax Syntax();
    Block? Body { get; }
    IExpression? ExpressionBody { get; }
}

public interface ILambdaExpression<T> : ILambdaExpression, IAnonymousFunctionExpression<T>
    where T : LambdaExpressionSyntax
{
    new T Syntax();
}