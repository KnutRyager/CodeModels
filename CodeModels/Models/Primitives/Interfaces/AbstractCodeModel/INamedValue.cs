using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Member;

namespace CodeModels.Models;

public interface INamedValue : INamed
{
    Modifier Modifier { get; }
    IType Type { get; }
    IExpression Value { get; }
    Property ToProperty();
}
