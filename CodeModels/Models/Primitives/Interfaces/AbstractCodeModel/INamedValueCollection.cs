using System.Collections.Generic;

namespace CodeModels.Models;

public interface INamedValueCollection<T> :
    IToClassConvertible,
    IToRecordConvertible,
    IToTupleConvertible
    where T : class, INamedValue
{
    /// <summary>
    /// Most general type.
    /// </summary>
    IType BaseType();

    List<IExpression> ToExpressions();

    ExpressionCollection ToValueCollection();
}
