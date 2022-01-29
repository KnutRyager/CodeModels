using System;
using static CodeAnalyzation.Models.CodeModelFactory;
using System.Collections.Generic;

namespace CodeAnalyzation.Models;

public class ProgramModelExecutionContext : IProgramModelExecutionContext
{
    private List<IProgramModelExecutionScope> _scopes = new List<IProgramModelExecutionScope>();
    private bool _continueFlag, _breakFlag, _returnFlag, _throwFlag;
    private IExpression? _returnValue;

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

    public void DefineVariable(string identifier) => GetScope().DefineVariable(identifier);

    public IExpression GetValue(string identifier) => FindScopeOrCrash(identifier).GetValue(identifier);

    public IExpression? GetValue(IdentifierExpression identifier) => GetValue(identifier.Name);

    public bool HandleBreak()
    {
        var wasBreak = _breakFlag;
        _breakFlag = false;
        return wasBreak;
    }

    public bool HandleContinue()
    {
        var wasContinue = _continueFlag;
        _continueFlag = false;
        return wasContinue;
    }

    public bool HandleReturn()
    {
        var wasReturn = _returnFlag;
        _returnFlag = false;
        return wasReturn;
    }

    public bool HandleThrow()
    {
        var wasThrow = _throwFlag;
        _throwFlag = false;
        return wasThrow;
    }

    public void SetBreak()
    {
        _breakFlag = true;
    }

    public void SetContinue()
    {
        _continueFlag = true;
    }

    public void SetReturn(IExpression Value)
    {
        _returnFlag = true;
    }

    public void SetValue(string identifier, IExpression value, bool define = false)
    {
        var scope = FindScope(identifier) ?? (define ? GetScope() : throw new ProgramModelExecutionException($"Identifier not found: {identifier}"));
        if (define) scope.DefineVariable(identifier);
        scope.SetValue(identifier, value);
    }

    public void SetValue(IdentifierExpression identifier, IExpression value, bool define = false)
    {
        SetValue(identifier.Name, value, define);
    }

    public void SetValue(IExpression expression, IExpression value, bool allowDefine = false)
    {
        switch (expression)
        {
            case IdentifierExpression identifier: SetValue(identifier, value, allowDefine); break;
            default: throw new NotImplementedException();
        }
    }

    public void Throw(IExpression value)
    {
        throw new NotImplementedException();
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

    public void Throw(Exception exception) => Throw(Literal(exception));
}
