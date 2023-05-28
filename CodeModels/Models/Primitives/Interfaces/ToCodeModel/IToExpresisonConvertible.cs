using CodeModels.Models.Primitives.Expression.Abstract;

namespace CodeModels.Models;

public interface IToExpresisonConvertible : IToArgumentConvertible
{
    IExpression ToExpression();
}
