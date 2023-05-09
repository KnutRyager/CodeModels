namespace CodeAnalyzation.Models;

public interface IAssigner
{
    void Assign(IExpression instance, IExpression value, IProgramModelExecutionContext context);
}
