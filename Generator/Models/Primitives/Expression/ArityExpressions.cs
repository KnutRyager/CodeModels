using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Models.CodeModelFactory;


namespace CodeAnalyzation.Models;

public record AnyArgExpression<T>(List<IExpression> Inputs, IType Type, OperationType Operation) : Expression<T>(Type) where T : ExpressionSyntax
{
    public override IEnumerable<ICodeModel> Children()
    {
        foreach (var input in Inputs) yield return input;
        yield return Type;
    }

    public override object? Evaluate(IProgramModelExecutionContext context) =>Operation.ApplyOperator(Inputs);

    public override T Syntax() => (T)Operation.Syntax(Inputs, Type);
}

public record UnaryExpression(IExpression Lhs, IType Type, OperationType Operation)
    : AnyArgExpression<ExpressionSyntax>(List(Lhs), Type, Operation);

public record BinaryExpression(IExpression Lhs, IExpression Rhs, IType Type, OperationType Operation)
    : AnyArgExpression<ExpressionSyntax>(List(Lhs, Rhs), Type, Operation);

public record TernaryExpression(IExpression Input, IExpression Output1, IExpression Output2, IType Type, OperationType Operation)
    : AnyArgExpression<ExpressionSyntax>(List(Input, Output1, Output2), Type, Operation);
