using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Execution.Scope;
using CodeModels.Models.Primitives.Expression.Abstract;

namespace CodeModels.Models.Interfaces;

public interface IAssignable
{
    void Assign(IExpression value, ICodeModelExecutionContext context, IList<ICodeModelExecutionScope> scopes);
}
