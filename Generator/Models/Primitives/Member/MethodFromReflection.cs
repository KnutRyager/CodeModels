using System.Reflection;

namespace CodeAnalyzation.Models;

public record MethodFromReflection(MethodInfo Method)
    : Method(Method.Name, new PropertyCollection(Method.GetParameters()), new TypeFromReflection(Method.ReturnType));
