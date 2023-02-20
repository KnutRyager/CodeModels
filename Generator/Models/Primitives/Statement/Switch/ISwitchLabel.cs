using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public interface ISwitchLabel : ICodeModel
{
    bool Match(IProgramModelExecutionContext context, IExpression condition);
    new SwitchLabelSyntax Syntax();
}

public interface ISwitchLabel<T> : ISwitchLabel, ICodeModel<T>
    where T : SwitchLabelSyntax
{
    new T Syntax();
}