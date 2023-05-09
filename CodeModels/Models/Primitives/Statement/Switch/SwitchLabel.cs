using CodeModels.Execution;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public abstract record SwitchLabel<T>()
    : CodeModel<T>, ISwitchLabel<T>
    where T : SwitchLabelSyntax
{
    public abstract bool Match(IProgramModelExecutionContext context, IExpression condition);

    SwitchLabelSyntax ISwitchLabel.Syntax() => Syntax();
}