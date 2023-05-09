using System;
using System.Linq;
using System.Reflection;
using CodeModels.Execution.Context;
using CodeModels.Models;
using static CodeModels.Factory.CodeModelFactory;

namespace CodeModels.Execution.Scope;

public class StaticExecutionScope : ICodeModelExecutionScope
{
    private Type _type;
    private ICodeModelExecutionContext _context;

    public StaticExecutionScope(ICodeModelExecutionContext context, Type type)
    {
        _context = context;
        _type = type;
    }

    public void DefineVariable(string identifier, IExpression? value = null) => throw new Exception("Can't define a variable in a static scope");
    public void DefineVariable(IdentifierExpression identifier, IExpression? value = null) => throw new Exception("Can't define a variable in a static scope");

    public IExpression GetValue(string identifier) => _type.GetMember(identifier).FirstOrDefault() switch
    {
        FieldInfo field => Literal(field.GetValue(null)),
        PropertyInfo property => Literal(property.GetValue(null)),
        MethodInfo _ => throw new CodeModelExecutionException($"Cannot get value of method '{identifier}'"),
        _ => throw new CodeModelExecutionException($"Cannot get non-found identifier '{identifier}'")
    };

    public IExpression GetValue(IdentifierExpression identifier) => GetValue(identifier.Name);

    public bool HasIdentifier(string identifier) => _type.GetMember(identifier).Length > 0;
    public bool HasIdentifier(IdentifierExpression identifier) => HasIdentifier(identifier.Name);

    public bool IsBottomScope() => true;

    public IExpression ExecuteMethod(string identifier, params IExpression[] parameters)
        => Literal(ExecuteMethodPlain(identifier, parameters.Select(x => x.EvaluatePlain(_context)).ToArray()));

    public object ExecuteMethodPlain(string identifier, params object?[] parameters) => _type.GetMethod(identifier) switch
    {
        MethodInfo method => method.Invoke(null, parameters),
        _ => throw new CodeModelExecutionException($"Cannot get non-found method '{identifier}'")
    };

    public void SetValue(string identifier, IExpression value)
    {
        switch (_type.GetMember(identifier).FirstOrDefault())
        {
            case FieldInfo field:
                field.SetValue(null, value.EvaluatePlain(_context));
                break;
            case PropertyInfo property:
                property.SetValue(null, value.EvaluatePlain(_context));
                break;
            case MethodInfo _:
                throw new CodeModelExecutionException($"Cannot set value of method '{identifier}'");
            default:
                throw new CodeModelExecutionException($"Cannot set non-found identifier '{identifier}'");
        };
    }

    public void SetValue(IdentifierExpression identifier, IExpression value) => SetValue(identifier.Name, value);

    public void Throw(IExpression value) => throw new NotImplementedException();

    public bool HasThis() => false;
    public IExpression This() => throw new CodeModelExecutionException($"No 'this' reference for static class.");
}