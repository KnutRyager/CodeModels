using System;
using System.Linq;
using System.Reflection;
using static CodeModels.Models.CodeModelFactory;

namespace CodeModels.Models;

public class ObjectModelExecutionScope : IProgramModelExecutionScope
{
    private object _object;
    private Type _type;
    private IProgramModelExecutionContext _context;

    public ObjectModelExecutionScope(IProgramModelExecutionContext context, object @object)
    {
        _context = context;
        _object = @object;
        _type = @object.GetType();
    }

    public void DefineVariable(string identifier, IExpression? value = null) => throw new Exception("Can't define a variable in an object scope");
    public void DefineVariable(IdentifierExpression identifier, IExpression? value = null) => throw new Exception("Can't define a variable in an object scope");

    public IExpression GetValue(string identifier) => _type.GetMember(identifier).FirstOrDefault() switch
    {
        FieldInfo field => Literal(field.GetValue(_object)),
        PropertyInfo property => Literal(property.GetValue(_object)),
        MethodInfo _ => throw new ProgramModelExecutionException($"Cannot get value of method '{identifier}'"),
        _ => throw new ProgramModelExecutionException($"Cannot get non-found identifier '{identifier}'")
    };

    public IExpression GetValue(IdentifierExpression identifier) => GetValue(identifier.Name);

    public bool HasIdentifier(string identifier) => _type.GetMember(identifier).Length > 0;
    public bool HasIdentifier(IdentifierExpression identifier) => HasIdentifier(identifier.Name);

    public bool IsBottomScope() => true;

    public IExpression ExecuteMethod(string identifier, params IExpression[] parameters) 
        => Literal(ExecuteMethodPlain(identifier, parameters.Select(x => x.EvaluatePlain(_context)).ToArray()));

    public object ExecuteMethodPlain(string identifier, params object?[] parameters) => _type.GetMethod(identifier) switch
    {
        MethodInfo method => method.Invoke(_object, parameters),
        _ => throw new ProgramModelExecutionException($"Cannot get non-found method '{identifier}'")
    };

    public void SetValue(string identifier, IExpression value)
    {
        switch (_type.GetMember(identifier).FirstOrDefault())
        {
            case FieldInfo field:
                field.SetValue(_object, value.EvaluatePlain(_context));
                break;
            case PropertyInfo property:
                property.SetValue(_object, value.EvaluatePlain(_context));
                break;
            case MethodInfo _:
                throw new ProgramModelExecutionException($"Cannot set value of method '{identifier}'");
            default:
                throw new ProgramModelExecutionException($"Cannot set non-found identifier '{identifier}'");
        };
    }

    public void SetValue(IdentifierExpression identifier, IExpression value) => SetValue(identifier.Name, value);

    public void Throw(IExpression value) => throw new NotImplementedException();

    public bool HasThis() => true;
    public IExpression This() => Literal(_object);
}