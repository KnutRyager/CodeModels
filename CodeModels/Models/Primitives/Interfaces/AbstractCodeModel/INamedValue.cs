using CodeModels.Models.Primitives.Expression.Abstract;

namespace CodeModels.Models;

public interface INamedValue 
    : INamed, IToPropertyConvertible, IToArgumentConvertible
{
    Modifier Modifier { get; }
    IType Type { get; }
    IExpression Value { get; }
}
