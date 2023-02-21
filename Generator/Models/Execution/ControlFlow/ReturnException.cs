namespace CodeAnalyzation.Models.Execution.Controlflow;

public class ReturnException : ControlFlowException
{
    public object? Value { get; private set; }

    public ReturnException(object? value)
    {
        this.Value = value;
    }
}
