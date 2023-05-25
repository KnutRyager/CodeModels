using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public record CaseSwitchLabel(IExpression Value)
    : SwitchLabel<CaseSwitchLabelSyntax>
{
    public static CaseSwitchLabel Create(IExpression value) => new(value);

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Value;
    }

    public override bool Match(ICodeModelExecutionContext context, IExpression condition)
        => Equals(Value.Evaluate(context).LiteralValue(), condition.LiteralValue());

    public override CaseSwitchLabelSyntax Syntax()
        => SyntaxFactory.CaseSwitchLabel(Value.Syntax());
}