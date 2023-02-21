using System;

namespace CodeAnalyzation.Models.Execution.ControlFlow;

public abstract class ControlFlowException : Exception
{
    public ControlFlowException() { }
    public ControlFlowException(Exception e) : base("", e) { }
}
