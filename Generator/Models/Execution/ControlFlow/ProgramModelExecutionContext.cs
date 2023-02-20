using System;

namespace CodeAnalyzation.Models.Execution.Controlflow;

public abstract class ControlFlowException : Exception
{
    public ControlFlowException() { }
    public ControlFlowException(Exception e) : base("", e) { }
}
