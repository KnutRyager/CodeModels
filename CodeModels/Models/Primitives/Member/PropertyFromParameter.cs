using System.Reflection;
using CodeModels.Factory;

namespace CodeModels.Models;

public record PropertyFromParameter(ParameterInfo Parameter)
    : AbstractProperty(new TypeFromReflection(Parameter.ParameterType), Parameter.Name, Parameter.HasDefaultValue ? CodeModelFactory.Literal(Parameter.DefaultValue) : null, Modifier.None);
