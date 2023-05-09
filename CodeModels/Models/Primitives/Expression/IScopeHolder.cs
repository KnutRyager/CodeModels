using System.Collections.Generic;
using CodeModels.Execution;

namespace CodeModels.Models;

public interface IScopeHolder
{
    IList<IProgramModelExecutionScope> GetScopes(IProgramModelExecutionContext context);
}
