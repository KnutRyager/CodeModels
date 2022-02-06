using System.Reflection;
using CodeAnalyzation.Reflection;
using Common.Reflection;
using Common.Util;
using Microsoft.CodeAnalysis;

namespace CodeAnalyzation.Models;

public record PropertyFromReflection(PropertyInfo Property)
    : Property(new TypeFromReflection(Property.PropertyType), Property.Name, ReflectionUtil.IsStatic(Property) ? new LiteralExpression(Property.GetValue(null)) : null,
        (!Property.CanWrite ? Modifier.Readonly : Modifier.None).SetFlags(Modifier.Public | Modifier.Property))
{
    public PropertyFromReflection(IPropertySymbol symbol) : this(SemanticReflection.GetProperty(symbol)) { }
}

