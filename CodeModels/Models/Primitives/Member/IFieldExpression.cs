using System.Collections.Generic;
using CodeModels.Execution.Scope;
using CodeModels.Models.Interfaces;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis;

namespace CodeModels.Models;

public interface IFieldExpression : IInvocation, IAssignable, IMemberAccess
{
    IField Field { get; }
    IList<ICodeModelExecutionScope>? Scopes { get; }
    ISymbol? Symbol { get; }
    AssignmentExpression Assign(IExpression value);
}

public interface IFieldExpression<T> : IFieldExpression where T : IField
{
    new T Field { get; }
}
