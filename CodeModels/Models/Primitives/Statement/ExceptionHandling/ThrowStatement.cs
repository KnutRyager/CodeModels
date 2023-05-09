using System.Collections.Generic;
using CodeModels.Execution.Context;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Generation.SyntaxFactoryCustom;

namespace CodeModels.Models;

public record ThrowStatement(IExpression Expression) : AbstractStatement<ThrowStatementSyntax>
{
    public override ThrowStatementSyntax Syntax() => ThrowStatementCustom(Expression.Syntax());
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Expression;
    }

    public override void Evaluate(ICodeModelExecutionContext context) => context.Throw(Expression.Evaluate(context));
}

public record ThrowExpression(IExpression Expression) : Expression<ThrowExpressionSyntax>(Expression.Get_Type())
{
    public override ThrowExpressionSyntax Syntax() => ThrowExpressionCustom(Expression.Syntax());
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Expression;
    }

    public override IExpression Evaluate(ICodeModelExecutionContext context)
    {
        context.Throw(Expression);
        return Expression;
    }
}
