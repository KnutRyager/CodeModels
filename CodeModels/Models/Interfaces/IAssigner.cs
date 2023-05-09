using CodeModels.Execution.Context;
using CodeModels.Models.Primitives.Expression.Abstract;

namespace CodeModels.Models.Interfaces;

public interface IAssigner
{
    void Assign(IExpression instance, IExpression value, ICodeModelExecutionContext context);
}
