using System;
using System.Collections.Generic;

namespace CodeAnalyzation.Models;

public class ProgramModelExecutionScope : IProgramModelExecutionScope
{
    private HashSet<object> _variables = new HashSet<object>();
    private IDictionary<object, IExpression> _values = new Dictionary<object, IExpression>();
    private IExpression? _this;

    public ProgramModelExecutionScope(IExpression? thisExpression = null)
    {
        _this = thisExpression;
    }

    public void DefineVariable(string identifier, IExpression? value = null)
    {
        if (_variables.Contains(identifier)) throw new ProgramModelExecutionException($"Already defined: {identifier}");
        _variables.Add(identifier);
        if (value is not null) SetValue(identifier, value);
    }
    public void DefineVariable(IdentifierExpression identifier, IExpression? value = null)
        => DefineVariable(identifier.Name, value);

    public IExpression GetValue(string identifier) => _values[identifier];
    public IExpression GetValue(IdentifierExpression identifier) => _values[identifier];

    public bool HasIdentifier(string identifier) => _variables.Contains(identifier);
    public bool HasIdentifier(IdentifierExpression identifier) => _variables.Contains(identifier);

    public IExpression ExecuteMethod(string identifier, params IExpression[] parameters)
    {
        throw new System.NotImplementedException();
    }

    public object ExecuteMethodPlain(string identifier, params object?[] parameters)
    {
        throw new System.NotImplementedException();
    }

    public bool IsBottomScope()
    {
        throw new System.NotImplementedException();
    }

    public void SetValue(string identifier, IExpression value) => _values[identifier] = value;
    public void SetValue(IdentifierExpression identifier, IExpression value) => _values[identifier] = value;

    public void Throw(IExpression value)
    {
        throw new System.NotImplementedException();
    }

    public void SetThis(IExpression thisExpression) => _this = thisExpression;

    public bool HasThis() => _this is not null;
    public IExpression This() => _this ?? throw new ProgramModelExecutionException($"No 'this' reference found.");

    public override string ToString()
        => $"ProgramModelScope. Values: {string.Join(Environment.NewLine, _values)}, Variables: {string.Join(Environment.NewLine, _variables)}";
}