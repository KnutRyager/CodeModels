using System.Collections.Generic;
using CodeModels.Execution.Scope;
using Microsoft.CodeAnalysis;

namespace CodeModels.Models;

public interface IPropertyModelExpression : IInvocation, IAssignable, IMemberAccess
{
    PropertyModel Property { get; }
    //IExpression? Instance { get; }
    IList<ICodeModelExecutionScope>? Scopes { get; }
    ISymbol? Symbol { get; }
}
