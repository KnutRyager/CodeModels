using System.Collections.Generic;
using CodeModels.AbstractCodeModels.Collection;
using CodeModels.Models.Primitives.Expression.Abstract;

namespace CodeModels.Models;

public interface INamedValueCollection<T> :
    IToParameterListConvertible,
    IToArgumentListConvertible,
    IToClassConvertible,
    IToRecordConvertible,
    IToTupleConvertible
    where T : class, INamed
{
    /// <summary>
    /// Most general type.
    /// </summary>
    IType BaseType();

    List<IExpression> ToExpressions();

    ExpressionCollection ToValueCollection();
}
