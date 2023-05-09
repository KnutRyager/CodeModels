using System.Reflection;

namespace CodeModels.Models;

public record PropertyFromParameter(ParameterInfo Parameter)
    : AbstractProperty(new TypeFromReflection(Parameter.ParameterType), Parameter.Name, Parameter.HasDefaultValue ? new LiteralExpression(Parameter.DefaultValue) : null, Modifier.None);
