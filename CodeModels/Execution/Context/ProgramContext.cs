using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using CodeModels.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Execution.Context;

public record ProgramContext(SemanticModel? Model = null) : IProgramContext
{
    private static IDictionary<IAssemblySymbol, IProgramContext> _contexts = new ConcurrentDictionary<IAssemblySymbol, IProgramContext>(SymbolEqualityComparer.Default);
    public static IProgramContext? Context { get; private set; }
    public static IProgramContext NewContext(CompilationUnitSyntax? compilation = null, SemanticModel? model = null)
    {
        var assembly = compilation is null ? null : model?.GetDeclaredSymbol(compilation)?.ContainingAssembly;
        var newContext = new ProgramContext(model);
        if (assembly is not null)
        {
            Context = newContext;
            _contexts[assembly] = Context;
        }
        return newContext;
    }

    public static IProgramContext? GetContext(ISymbol symbol) => _contexts[symbol.ContainingAssembly];
    public static IProgramContext? GetContext(SemanticModel? model)
        => GetContext(model.GetDeclaredSymbol(model.SyntaxTree.GetCompilationUnitRoot()));

    private Dictionary<ISymbol, ICodeModel> _symbols = new(SymbolEqualityComparer.Default);

    public bool Contains(ISymbol symbol)
    {
        throw new NotImplementedException();
    }

    public bool Contains(SyntaxNode node)
        => Contains(GetSymbol(node));

    public T Get<T>(ISymbol symbol) where T : class, ICodeModel
        => _symbols[symbol] as T ?? throw new NotImplementedException();// Register(symbol, SemanticReflection.GetMemberInfo(symbol));

    public T Get<T>(SyntaxNode node) where T : class, ICodeModel
    => Get<T>(GetSymbol(node));

    public T? TryGet<T>(ISymbol symbol) where T : class, ICodeModel
            => _symbols.ContainsKey(symbol) ? _symbols[symbol] as T : null;
    public T? TryGet<T>(SyntaxNode node) where T : class, ICodeModel
        => TryGet<T>(GetSymbol(node));

    public IExpression GetSingleton(IType type)
    {
        throw new NotImplementedException();
    }

    public T Register<T>(ISymbol symbol, T model) where T : ICodeModel
    {
        if (symbol is IFieldSymbol f)
        {

        }
        _symbols[symbol] = model;
        return model;
    }


    public T Register<T>(SyntaxNode node, T model) where T : ICodeModel
        => Register(GetSymbol(node), model);

    private ISymbol GetSymbol(SyntaxNode node)
        => Model is null ? throw new ArgumentException("No semantic model.")
    : Model.GetSymbolInfo(node).Symbol ?? Model.GetDeclaredSymbol(node)
         ?? throw new ArgumentException("No symbol.");
}
