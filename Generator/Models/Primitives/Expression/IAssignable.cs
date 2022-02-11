namespace CodeAnalyzation.Models;

public interface IAssignable
{
    void Assign(IExpression value, IProgramModelExecutionContext context);
}
