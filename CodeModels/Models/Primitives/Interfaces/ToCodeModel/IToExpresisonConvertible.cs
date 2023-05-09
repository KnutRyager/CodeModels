using CodeModels.Models.Primitives.Expression.Abstract;

namespace CodeModels.Models;

public interface IToExpresisonConvertible
{
    IExpression ToExpression();
}
