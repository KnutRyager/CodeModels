using System.Collections.Generic;

namespace CodeAnalyzation.Models;

public class ProgramModelExecutionScope : IProgramModelExecutionScope
{
    private HashSet<object> _variables = new HashSet<object>();
    private IDictionary<object, IExpression> _values = new Dictionary<object, IExpression>();

    public void DefineVariable(string identifier)
    {
        if (_variables.Contains(identifier)) throw new ProgramModelExecutionException($"Already defined: {identifier}");
        _variables.Add(identifier);
    }
    public void DefineVariable(IdentifierExpression identifier)
    {
        if (_variables.Contains(identifier)) throw new ProgramModelExecutionException($"Already defined: {identifier}");
        _variables.Add(identifier);
    }

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
}
