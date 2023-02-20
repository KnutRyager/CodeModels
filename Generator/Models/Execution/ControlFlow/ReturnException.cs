namespace CodeAnalyzation.Models.Execution.Controlflow;

public class ReturnException : ControlFlowException
{
    public IExpression Expression { get; private set; }

    public ReturnException(IExpression expression)
    {
        this.Expression = expression;
    }
}
