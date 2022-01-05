using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
//using Newtonsoft.Json;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static CodeAnalyzation.Models.CodeModelFactory;


namespace CodeAnalyzation.Models
{
    public record AnyArgExpression(List<IExpression> Inputs, IType Type, OperationType Operation) : Expression<ExpressionSyntax>(Type)
    {
        public override ExpressionSyntax Syntax() => Operation.Syntax(Inputs, Type);
    }

    public record UnaryExpression(IExpression Lhs, IType Type, OperationType Operation)
        : AnyArgExpression(List(Lhs), Type, Operation);

    public record BinaryExpression(IExpression Lhs, IExpression Rhs, IType Type, OperationType Operation)
        : AnyArgExpression(List(Lhs, Rhs), Type, Operation);

    public record TernaryExpression(IExpression Input, IExpression Output1, IExpression Output2, IType Type, OperationType Operation)
        : AnyArgExpression(List(Input, Output1, Output2), Type, Operation);
}