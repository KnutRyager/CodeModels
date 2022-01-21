using System.Collections.Generic;

namespace CodeAnalyzation.Models;

public class ProgramModelExecutionContext : IProgramModelExecutionContext
{
    private List<IProgramModelExecutionScope> _scopes = new List<IProgramModelExecutionScope>();

    public void EnterScope(object owner)
    {
        if (_scopes.Count >= 10000) throw new ProgramModelExecutionException("Stackoverflow");
        throw new System.NotImplementedException();
    }

    public void EnterScope()
    {
        if (_scopes.Count >= 10000) throw new ProgramModelExecutionException("Stackoverflow");
        _scopes.Add(new ProgramModelExecutionScope());
    }

    public void ExitScope(object owner)
    {
        if (_scopes.Count == 0) throw new ProgramModelExecutionException("Popped scope when no scopes");
    }

    public void ExitScope()
    {
        if (_scopes.Count == 0) throw new ProgramModelExecutionException("Popped scope when no scopes");
        _scopes.RemoveAt(_scopes.Count - 1);
    }

    public object? GetValue(string identifier) => FindScopeOrCrash(identifier).GetValue(identifier);

    public object? GetValue(IdentifierExpression identifier)
    {
        throw new System.NotImplementedException();
    }

    public void SetValue(string identifier, IExpression value, bool define = false)
    {
        var scope = FindScope(identifier) ?? (define ? GetScope() : throw new ProgramModelExecutionException($"Identifier not found: {identifier}"));
        if (define) scope.DefineVariable(identifier);
        scope.SetValue(identifier, value);
    }

    public void SetValue(IdentifierExpression identifier, IExpression value, bool define = false)
    {
        throw new System.NotImplementedException();
    }

    public void Throw(IExpression value)
    {
        throw new System.NotImplementedException();
    }

    private IProgramModelExecutionScope? FindScope(string identifier)
    {
        for (var i = 0; i < _scopes.Count; i++)
        {
            var scope = GetScope(i);
            if (scope.HasIdentifier(identifier)) return scope;
        }
        return default;
    }

    private IProgramModelExecutionScope FindScopeOrCrash(string identifier) => FindScope(identifier) ?? throw new ProgramModelExecutionException($"Cannot find scope of identifier: {identifier}");

    private IProgramModelExecutionScope GetScope(int depth = 0) => depth < _scopes.Count ? _scopes[_scopes.Count - 1 - depth] : throw new ProgramModelExecutionException($"No scope at depth: {depth}");
}
