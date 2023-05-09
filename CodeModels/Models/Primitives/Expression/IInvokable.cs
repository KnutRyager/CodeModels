using System.Collections.Generic;

namespace CodeModels.Models;

public interface IInvokable
{
    IInvocation Invoke(IExpression? caller, IEnumerable<IExpression> arguments);
}

public interface IInvokable<T> : IInvokable where T : IInvocation
{
    new T Invoke(IExpression? caller, IEnumerable<IExpression> arguments);
}
