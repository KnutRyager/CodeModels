using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace CodeAnalyzation.Models;

public interface IFieldModelExpression : IInvocation, IAssignable, IMemberAccess
{
    FieldModel Field { get; }
    //IExpression? Instance { get; }
    IList<IProgramModelExecutionScope>? Scopes { get; }
    ISymbol? Symbol { get; }
}
