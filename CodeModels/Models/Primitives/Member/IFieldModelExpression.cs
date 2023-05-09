using System.Collections.Generic;
using CodeModels.Execution.Scope;
using CodeModels.Models.Interfaces;
using Microsoft.CodeAnalysis;

namespace CodeModels.Models;

public interface IFieldModelExpression : IInvocation, IAssignable, IMemberAccess
{
    FieldModel Field { get; }
    //IExpression? Instance { get; }
    IList<ICodeModelExecutionScope>? Scopes { get; }
    ISymbol? Symbol { get; }
}
