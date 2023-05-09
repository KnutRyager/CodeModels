using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Execution.Scope;

namespace CodeModels.Models;

public interface IScopeHolder
{
    IList<ICodeModelExecutionScope> GetScopes(ICodeModelExecutionContext context);
}
