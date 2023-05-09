using CodeModels.Execution.Context;

namespace CodeModels.Models;

public interface IAssigner
{
    void Assign(IExpression instance, IExpression value, ICodeModelExecutionContext context);
}
