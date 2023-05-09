using System;
using System.Collections.Generic;
using CodeModels.Models;

namespace CodeModels.Execution;

public class ProgramModelExecutionScope : IProgramModelExecutionScope
{
    private HashSet<object> _variables = new HashSet<object>();
    private IDictionary<object, IExpression> _values = new Dictionary<object, IExpression>();
    private IExpression? _this;
    private IDictionary<string, string> _aliases;

    public ProgramModelExecutionScope(IExpression? thisExpression = null, IDictionary<string, string>? aliases = null)
    {
        _this = thisExpression;
        _aliases = aliases ?? new Dictionary<string, string>();
    }

    public void DefineVariable(string identifier, IExpression? value = null)
    {
        identifier = Unalias(identifier);
        if (_variables.Contains(identifier)) throw new ProgramModelExecutionException($"Already defined: {identifier}");
        _variables.Add(identifier);
        if (value is not null) SetValue(identifier, value);
    }
    public void DefineVariable(IdentifierExpression identifier, IExpression? value = null)
        => DefineVariable(identifier.Name, value);
    public void DefineAlias(string identifier, string alias)
    {
        _aliases[alias] = identifier;
        //_aliases[identifier] = alias;
    }

    public IExpression GetValue(string identifier) => _values[Unalias(identifier)];
    public IExpression GetValue(IdentifierExpression identifier) => GetValue(identifier.Name);

    public bool HasIdentifier(string identifier) => _variables.Contains(Unalias(identifier));
    public bool HasIdentifier(IdentifierExpression identifier) => HasIdentifier(identifier.Name);

    public IExpression ExecuteMethod(string identifier, params IExpression[] parameters)
    {
        throw new NotImplementedException();
    }

    public object ExecuteMethodPlain(string identifier, params object?[] parameters)
    {
        throw new NotImplementedException();
    }

    public bool IsBottomScope()
    {
        throw new NotImplementedException();
    }

    public void SetValue(string identifier, IExpression value) => _values[Unalias(identifier)] = value;
    public void SetValue(IdentifierExpression identifier, IExpression value) => SetValue(identifier.Name, value);

    public void Throw(IExpression value)
    {
        throw new NotImplementedException();
    }

    public void SetThis(IExpression thisExpression) => _this = thisExpression;

    public bool HasThis() => _this is not null;
    public IExpression This() => _this ?? throw new ProgramModelExecutionException($"No 'this' reference found.");

    public override string ToString()
        => $"ProgramModelScope. Values: {string.Join(Environment.NewLine, _values)}, Variables: {string.Join(Environment.NewLine, _variables)}";

    private string Unalias(string identifier) => _aliases.ContainsKey(identifier) ? _aliases[identifier] : identifier;
}