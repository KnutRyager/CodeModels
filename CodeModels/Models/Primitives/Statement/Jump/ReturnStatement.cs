using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Execution.ControlFlow;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Generation.SyntaxFactoryCustom;

namespace CodeModels.Models;

public record ReturnStatement(IExpression Expression) : AbstractStatement<ReturnStatementSyntax>
{
    public override ReturnStatementSyntax Syntax() => ReturnCustom(Expression.Syntax());

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Expression;
    }

    public override void Evaluate(ICodeModelExecutionContext context)
        => throw new ReturnException(Expression.EvaluatePlain(context));
}
