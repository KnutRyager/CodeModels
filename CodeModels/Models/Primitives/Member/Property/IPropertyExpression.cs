using System.Collections.Generic;
using CodeModels.Execution.Scope;
using CodeModels.Models.Interfaces;
using Microsoft.CodeAnalysis;

namespace CodeModels.Models;

public interface IPropertyExpression : IInvocation, IAssignable, IMemberAccess
{
    IProperty Property { get; }
    IList<ICodeModelExecutionScope>? Scopes { get; }
    ISymbol? Symbol { get; }
}
