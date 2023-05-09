using System;
using CodeModels.Models.Primitives.Expression.Abstract;
using Common.DataStructures;
using Common.Reflection;

namespace CodeModels.Models;

public record TypeFromReflection(Type ReflectedType, bool Required = true, bool IsMulti = false)   // TODO: Generics
    : AbstractType(ReflectionSerialization.GetToShortHandName(ReflectedType.Name), new EqualityList<IType>(), Required, IsMulti, ReflectedType), IExpression
{
    public override IType PlainType()
    {
        throw new NotImplementedException();
    }
}
