using System.Collections.Generic;

namespace CodeModels.Models;

public interface IAssignable
{
    void Assign(IExpression value, IProgramModelExecutionContext context, IList<IProgramModelExecutionScope> scopes);
}
