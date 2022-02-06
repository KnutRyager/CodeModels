using System.Reflection;
using CodeAnalyzation.Reflection;
using Microsoft.CodeAnalysis;

namespace CodeAnalyzation.Models;

public record MethodFromReflection(MethodInfo Method)
    : Method(Method.Name, new PropertyCollection(Method.GetParameters()), new TypeFromReflection(Method.ReturnType))
{
    public MethodFromReflection(IMethodSymbol symbol) : this(SemanticReflection.GetMethod(symbol)) { }
}

