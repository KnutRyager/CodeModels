using System.Reflection;
using CodeModels.AbstractCodeModels;
using CodeModels.Factory;

namespace CodeModels.Models.Primitives.Member;

public record PropertyFromParameter(ParameterInfo Parameter)
    : AbstractProperty(TypeFromReflection.Create(Parameter.ParameterType), Parameter.Name, Parameter.HasDefaultValue ? CodeModelFactory.Literal(Parameter.DefaultValue) : null, Modifier.None);
