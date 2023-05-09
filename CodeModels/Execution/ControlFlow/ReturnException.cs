namespace CodeModels.Execution.ControlFlow;

public class ReturnException : ControlFlowException
{
    public object? Value { get; private set; }

    public ReturnException(object? value)
    {
        Value = value;
    }
}
