using System.Collections.Generic;

namespace CodeModels.Models;

public interface IScopeHolder
{
    IList<IProgramModelExecutionScope> GetScopes(IProgramModelExecutionContext context);
}
