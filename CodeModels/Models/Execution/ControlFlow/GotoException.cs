namespace CodeAnalyzation.Models.Execution.ControlFlow;

public class GotoException : ControlFlowException
{
    public string Label { get; private set; }

    public GotoException(string label) : base()
    {
        Label = label;
    }
}
