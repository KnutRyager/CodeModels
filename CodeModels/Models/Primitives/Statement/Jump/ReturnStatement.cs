using System.Collections.Generic;
using CodeModels.Models.Execution.ControlFlow;
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

    public override void Evaluate(IProgramModelExecutionContext context)
        => throw new ReturnException(Expression.EvaluatePlain(context));
}
