using System;

namespace CodeModels.Models.Execution.ControlFlow;

public class ThrowException : ControlFlowException
{
    public IExpression Expression { get; private set; }

    public ThrowException(IExpression expression)
    {
        Expression = expression;
    }

    public ThrowException(Exception exception) : base(exception)
    {
        Expression = new LiteralExpression(exception);
    }
}
