using System;

namespace CodeAnalyzation.Models.Execution.Controlflow;

public class ThrowException : ControlFlowException
{
    public IExpression Expression { get; private set; }

    public ThrowException(IExpression expression)
    {
        this.Expression = expression;
    }

    public ThrowException(Exception exception) : base(exception)
    {
        this.Expression = new LiteralExpression(exception);
    }
}
