using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace CodeModels.Models;

public interface IPropertyModelExpression : IInvocation, IAssignable, IMemberAccess
{
    PropertyModel Property { get; }
    //IExpression? Instance { get; }
    IList<IProgramModelExecutionScope>? Scopes { get; }
    ISymbol? Symbol { get; }
}
