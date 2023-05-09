using CodeModels.Execution.Context;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public interface ISwitchLabel : ICodeModel
{
    bool Match(ICodeModelExecutionContext context, IExpression condition);
    new SwitchLabelSyntax Syntax();
}

public interface ISwitchLabel<T> : ISwitchLabel, ICodeModel<T>
    where T : SwitchLabelSyntax
{
    new T Syntax();
}