using System;
using Common.DataStructures;
using Common.Reflection;

namespace CodeAnalyzation.Models;

public record TypeFromReflection(Type ReflectedType, bool Required = true, bool IsMulti = false)   // TODO: Generics
    : AbstractType(ReflectionSerialization.GetToShortHandName(ReflectedType.Name), new EqualityList<IType>(), Required, IsMulti, ReflectedType), IExpression
{
}
