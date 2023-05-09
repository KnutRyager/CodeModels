using System.Collections.Generic;
using CodeModels.Execution;

namespace CodeModels.Models;

public interface IAssignable
{
    void Assign(IExpression value, IProgramModelExecutionContext context, IList<IProgramModelExecutionScope> scopes);
}
