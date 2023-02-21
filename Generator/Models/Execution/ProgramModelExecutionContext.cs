using System;
using static CodeAnalyzation.Models.CodeModelFactory;
using System.Collections.Generic;
using CodeAnalyzation.Models.Execution.Controlflow;
using System.IO;
using Azure.Storage.Blobs.Models;

namespace CodeAnalyzation.Models;

public class ProgramModelExecutionContext : IProgramModelExecutionContext
{
    private List<IProgramModelExecutionScope> _scopes = new List<IProgramModelExecutionScope>();

    public IExpression PreviousExpression { get; private set; } = VoidValue;

    private TextWriter Console { get; set; }
    private int previousValueLock = 0;

    public ProgramModelExecutionContext(TextWriter? console = null)
    {
        Console = console ?? new StringWriter();
    }

    public string ConsoleOutput => Console.ToString();

    public void ConsoleWrite(string s)
    {
        Console.Write(s);
        SetPreviousExpression(new LiteralExpression(s));
    }

    public void ConsoleWriteLine(string s) => ConsoleWrite($"{s}\r\n");
    public void IncreaseDisableSetPreviousValueLock() => previousValueLock++;
    public void DecreaseDisableSetPreviousValueLock() => previousValueLock--;

    public void EnterScope(object owner)
    {
        if (_scopes.Count >= 10000) throw new ProgramModelExecutionException("Stackoverflow");
        _scopes.Add(new ObjectModelExecutionScope(this, owner));
    }

    public void EnterScope()
    {
        if (_scopes.Count >= 10000) throw new ProgramModelExecutionException("Stackoverflow");
        _scopes.Add(new ProgramModelExecutionScope());
    }

    public void ExitScope(object owner)
    {
        if (_scopes.Count == 0) throw new ProgramModelExecutionException("Popped scope when no scopes");
        ExitScope();    // TODO: Ensure enough scopes exited in case misaligned
    }

    public void ExitScope()
    {
        if (_scopes.Count == 0) throw new ProgramModelExecutionException("Popped scope when no scopes");
        _scopes.RemoveAt(_scopes.Count - 1);
    }

    public void DefineVariable(string identifier) => GetScope().DefineVariable(identifier);

    public IExpression GetValue(string identifier) => FindScopeOrCrash(identifier).GetValue(identifier);

    public IExpression? GetValue(IdentifierExpression identifier) => GetValue(identifier.Name);

    public void SetValue(string identifier, IExpression value, bool define = false)
    {
        var scope = FindScope(identifier) ?? (define ? GetScope() : throw new ProgramModelExecutionException($"Identifier not found: {identifier}"));
        if (define) scope.DefineVariable(identifier);
        scope.SetValue(identifier, value);
        SetPreviousExpression(value);
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
            case LiteralExpression _: break;
            default: throw new NotImplementedException();
        }
    }

    public IExpression ExecuteMethod(string identifier, params IExpression[] parameters)
    {
        var scope = FindScope(identifier) ?? throw new ProgramModelExecutionException($"Identifier not found: {identifier}");
        return scope.ExecuteMethod(identifier, parameters);
    }

    public object ExecuteMethodPlain(string identifier, params object?[] parameters)
    {
        var scope = FindScope(identifier) ?? throw new ProgramModelExecutionException($"Identifier not found: {identifier}");
        return scope.ExecuteMethodPlain(identifier, parameters);
    }

    public void Throw(IExpression value)
    {
        if (value.LiteralValue is Exception e) throw new ThrowException(e);
        throw new ThrowException(value);
    }

    public void Throw(Exception exception) => throw new ThrowException(exception);

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

    public IExpression SetPreviousExpression(IExpression expression)
    {
        if (previousValueLock <= 0)
        {
            PreviousExpression = expression;
        }
        return expression;
    }

    public IExpression This()
    {
        for (var i = 0; i < _scopes.Count; i++)
        {
            var scope = GetScope(i);
            if (scope.HasThis()) return scope.This();
        }
        throw new ProgramModelExecutionException($"No 'this' reference found.");
    }

    public override string ToString()
        => $"ProgramModelContext. Scopes: {string.Join(Environment.NewLine, _scopes)}";
}
