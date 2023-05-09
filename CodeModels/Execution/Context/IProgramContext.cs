using CodeModels.Models;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis;

namespace CodeModels.Execution.Context;

public interface IProgramContext
{
    IExpression GetSingleton(IType type);
    bool Contains(ISymbol symbol);
    bool Contains(SyntaxNode node);
    T Get<T>(ISymbol symbol) where T : class, ICodeModel;
    T Get<T>(SyntaxNode node) where T : class, ICodeModel;
    T? TryGet<T>(ISymbol symbol) where T : class, ICodeModel;
    T? TryGet<T>(SyntaxNode node) where T : class, ICodeModel;
    T Register<T>(ISymbol symbol, T model) where T : ICodeModel;
    T Register<T>(SyntaxNode node, T model) where T : ICodeModel;
}
