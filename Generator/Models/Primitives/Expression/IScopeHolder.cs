using System.Collections.Generic;

namespace CodeAnalyzation.Models;

public interface IScopeHolder
{
    IList<IProgramModelExecutionScope> GetScopes(IProgramModelExecutionContext context);
}
