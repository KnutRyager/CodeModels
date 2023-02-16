using System;
using System.Collections.Generic;
using CodeAnalyzation.Reflection;
using Microsoft.CodeAnalysis;

namespace CodeAnalyzation.Models.ProgramModels;

public interface IProgramContext
{
    IExpression GetSingleton(IType type);
    bool Contains(ISymbol symbol);
    bool Contains(SyntaxNode node);
    T Get<T>(ISymbol symbol) where T : class, ICodeModel;
    T Get<T>(SyntaxNode node) where T : class, ICodeModel;
    T Register<T>(ISymbol symbol, T model) where T : ICodeModel;
    T Register<T>(SyntaxNode node, T model) where T : ICodeModel;
}

public record ProgramContext(SemanticModel? Model = null) : IProgramContext
{
    public static IProgramContext? Context { get; private set; }
    public static void NewContext(SemanticModel? model = null) => Context = new ProgramContext(model);

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

    public IExpression GetSingleton(IType type)
    {
        throw new NotImplementedException();
    }

    public T Register<T>(ISymbol symbol, T model) where T : ICodeModel
    {
        throw new NotImplementedException();
    }


    public T Register<T>(SyntaxNode node, T model) where T : ICodeModel
        => Register(GetSymbol(node), model);

    private ISymbol GetSymbol(SyntaxNode node)
        => (Model?.GetSymbolInfo(node) ?? throw new ArgumentException("No semantic model.")).Symbol ?? throw new ArgumentException("No symbol model.");
}
