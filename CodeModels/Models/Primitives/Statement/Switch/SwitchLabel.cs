using CodeModels.Execution.Context;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public abstract record SwitchLabel<T>()
    : CodeModel<T>, ISwitchLabel<T>
    where T : SwitchLabelSyntax
{
    public abstract bool Match(ICodeModelExecutionContext context, IExpression condition);

    SwitchLabelSyntax ISwitchLabel.Syntax() => Syntax();
}