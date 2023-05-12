using System;
using CodeModels.Factory;
using CodeModels.Models.Primitives.Expression.Abstract;

namespace CodeModels.Execution.ControlFlow;

public class ThrowException : ControlFlowException
{
    public IExpression Expression { get; private set; }

    public ThrowException(IExpression expression)
    {
        Expression = expression;
    }

    public ThrowException(Exception exception) : base(exception)
    {
        Expression = CodeModelFactory.Literal(exception);
    }
}
