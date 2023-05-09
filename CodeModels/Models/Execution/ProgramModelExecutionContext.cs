using System;
using System.Collections.Generic;
using System.IO;
using CodeModels.Models.Execution;
using CodeModels.Models.Execution.ControlFlow;
using CodeModels.Utils;
using static CodeModels.Factory.CodeModelFactory;

namespace CodeModels.Models;

public class ProgramModelExecutionContext : IProgramModelExecutionContext
{
    public IProgramContext? ProgramContext { get; private set; }
    private List<IProgramModelExecutionScope> _scopes = new List<IProgramModelExecutionScope>();
    private readonly IDictionary<string, IMember> _members = new Dictionary<string, IMember>();
    //private IDictionary<string, ClassDeclaration> _types = new Dictionary<string, ClassDeclaration>();
    private IDictionary<string, IProgramModelExecutionScope> _staticScopes = new Dictionary<string, IProgramModelExecutionScope>();

    public IExpression PreviousExpression { get; private set; } = VoidValue;

    private TextWriter Console { get; set; }
    private int previousValueLock = 0;

    public ProgramModelExecutionContext(IProgramContext? programContext = null, TextWriter? console = null)
    {
        ProgramContext = programContext;
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

    public void EnterScope() => EnterScope(new ProgramModelExecutionScope());

    public void EnterScope(IProgramModelExecutionScope scope)
    {
        if (_scopes.Count >= 10000) throw new ProgramModelExecutionException("Stackoverflow");
        _scopes.Add(scope);
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

    public void EnterScopes(IEnumerable<IProgramModelExecutionScope>? scopes)
    {
        if (scopes is null) return;
        foreach (var scope in scopes)
        {
            EnterScope(scope);
        }
    }

    public void ExitScopes(IEnumerable<IProgramModelExecutionScope>? scopes)
    {
        if (scopes is null) return;
        foreach (var _ in scopes)
        {
            ExitScope();
        }
    }

    public void DefineVariable(string identifier) => GetScope().DefineVariable(identifier);

    public IExpression GetValue(string identifier) => FindScopeOrCrash(identifier).GetValue(identifier);

    public IExpression GetValue(IdentifierExpression identifier) => GetValue(identifier.Name);

    public ICodeModel GetValueOrMember(string identifier) => FindScope(identifier)?.GetValue(identifier) as ICodeModel ?? GetMember(identifier);

    public ICodeModel GetValueOrMember(IdentifierExpression identifier) => GetValueOrMember(identifier.Name);
    public ICodeModel? TryGetValueOrMember(string identifier) => FindScope(identifier)?.GetValue(identifier) as ICodeModel ?? TryGetMember(identifier);
    public ICodeModel? TryGetValueOrMember(IdentifierExpression identifier) => TryGetValueOrMember(identifier.Name);

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
            case MemberAccessExpression memberAccess:
                {
                    if (memberAccess.Expression is InstantiatedObject instance)
                    {
                        try
                        {
                            instance.EnterScopes(this);
                            SetValue(memberAccess.Identifier, value, allowDefine);
                        }
                        finally
                        {
                            instance.ExitScopes(this);
                        }
                    }
                    break;
                }
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
        if (value.LiteralValue() is Exception e) throw new ThrowException(e);
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

    public IProgramModelExecutionScope CaptureScope() => GetScope(0);

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

    public IMember GetMember(string? key, string? name = null) => _members[NamespaceUtils.GetKeyAndName(key, name)];
    public IMember? TryGetMember(string? key, string? name = null)
    {
        _members.TryGetValue(NamespaceUtils.GetKeyAndName(key, name), out var member);
        return member;
    }

    public void AddMember(string? key, IMember member)
    {
        _members[NamespaceUtils.GetKeyAndName(key, member.Name)] = member;
        //_staticScopes[member.Name] = member.GetStaticScope();
    }

    public override string ToString()
        => $"ProgramModelContext. Scopes: {string.Join(Environment.NewLine, _scopes)}";

    public void Register(string key, IMember type)
    {
        throw new NotImplementedException();
    }
}
