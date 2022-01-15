using System.Reflection;
using Common.Util;

namespace CodeAnalyzation.Models;

public record PropertyFromReflection(PropertyInfo Propery)
    : Property(new TypeFromReflection(Propery.PropertyType), Propery.Name, Propery.GetValue(null) != null ? new LiteralExpression(Propery.GetValue(null)) : null,
        (!Propery.CanWrite ? Modifier.Readonly : Modifier.None).SetFlags(Modifier.Public | Modifier.Property));
