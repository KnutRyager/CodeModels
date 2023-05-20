using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Execution.Scope;
using CodeModels.Models.Primitives.Expression.Abstract;

namespace CodeModels.Models.Interfaces;

public interface IAssigner
{
    AssignmentExpression Assign(IExpression? instance, IExpression value);
    void Assign(IExpression instance, IExpression value, ICodeModelExecutionContext context);
    void Assign(IExpression value, ICodeModelExecutionContext context, IList<ICodeModelExecutionScope> scopes);
}
