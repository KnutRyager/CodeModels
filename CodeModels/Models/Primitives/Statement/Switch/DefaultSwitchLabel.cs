using System.Collections.Generic;
using CodeModels.Execution.Context;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public record DefaultSwitchLabel()
    : SwitchLabel<DefaultSwitchLabelSyntax>
{
    public override IEnumerable<ICodeModel> Children()
    {
        yield break;
    }

    public override bool Match(ICodeModelExecutionContext context, IExpression condition)
        => true;

    public override DefaultSwitchLabelSyntax Syntax()
        => SyntaxFactory.DefaultSwitchLabel();
}