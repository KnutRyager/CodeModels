using System.Collections.Generic;
using CodeModels.Models.Primitives.Expression.Abstract;

namespace CodeModels.Models.Interfaces;

public interface IInvokable
{
    IInvocation Invoke(IExpression? caller, IEnumerable<IExpression> arguments);
}

public interface IInvokable<T> : IInvokable where T : IInvocation
{
    new T Invoke(IExpression? caller, IEnumerable<IExpression> arguments);
}
