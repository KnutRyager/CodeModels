using System.Reflection;

namespace CodeModels.Models;

public record PropertyFromParameter(ParameterInfo Parameter)
    : Property(new TypeFromReflection(Parameter.ParameterType), Parameter.Name, Parameter.HasDefaultValue ? new LiteralExpression(Parameter.DefaultValue) : null, Modifier.None);
