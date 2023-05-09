using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;


namespace CodeModels.Models;

public record AnyArgExpression<T>(List<IExpression> Inputs, IType Type, OperationType Operation) : Expression<T>(Type) where T : ExpressionSyntax
{
    public override IEnumerable<ICodeModel> Children()
    {
        foreach (var input in Inputs) yield return input;
        yield return Type;
    }

    public override IExpression Evaluate(IProgramModelExecutionContext context)
    {
        switch (Operation)
        {
            case OperationType.UnaryAddBefore:
            case OperationType.UnarySubtractBefore:
                {
                    var expression = Inputs.First();
                    var value = Literal(Operation.ApplyOperator(Inputs.Select(x => x.EvaluatePlain(context)).ToArray()));
                    context.SetValue(expression, value);
                    return value;
                }
            case OperationType.UnaryAddAfter:
            case OperationType.UnarySubtractAfter:
                {
                    var expression = Inputs.First();
                    var value = Literal(Operation.ApplyOperator(Inputs.Select(x => x.EvaluatePlain(context)).ToArray()));
                    context.SetValue(expression, value);
                    return expression;
                }
            default:
                return Literal(Operation.ApplyOperator(Inputs.Select(x => x.EvaluatePlain(context)).ToArray()));    // TODO: Handle object types
        }
    }

    public override T Syntax() => (T)Operation.Syntax(Inputs, Type);
}

public record UnaryExpression(IExpression Lhs, IType Type, OperationType Operation)
    : AnyArgExpression<ExpressionSyntax>(List(Lhs), Type, Operation);

public record BinaryExpression(IExpression Lhs, IExpression Rhs, IType Type, OperationType Operation)
    : AnyArgExpression<ExpressionSyntax>(List(Lhs, Rhs), Type, Operation);

public record TernaryExpression(IExpression Input, IExpression Output1, IExpression Output2, IType Type, OperationType Operation)
    : AnyArgExpression<ExpressionSyntax>(List(Input, Output1, Output2), Type, Operation);
