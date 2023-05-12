using CodeModels.Execution.Context;
using CodeModels.Models.Interfaces;
using CodeModels.Models.Primitives.Expression.Abstract;

namespace CodeModels.Models;

public interface IInstantiatedObject : IScopeHolder, IExpression
{
    IBaseTypeDeclaration Type { get; }
    void EnterScopes(ICodeModelExecutionContext context);
    void ExitScopes(ICodeModelExecutionContext context);
}

public interface IInstantiatedObject<T> : IInstantiatedObject where T : IBaseTypeDeclaration
{
    new T Type { get; }
}