﻿using System.Reflection;
using CodeAnalyzation.Reflection;
using Common.Extensions;
using Common.Reflection;
using Microsoft.CodeAnalysis;

namespace CodeAnalyzation.Models;

public record PropertyFromReflection(PropertyInfo Property)
    : Property(new TypeFromReflection(Property.PropertyType), Property.Name, ReflectionUtil.IsStatic(Property) ? new LiteralExpression(Property.GetValue(null)) : null,
        (!Property.CanWrite ? Modifier.Readonly : Modifier.None).SetFlags(Modifier.Public | Modifier.Property))
{
    public PropertyFromReflection(IPropertySymbol symbol) : this(SemanticReflection.GetProperty(symbol)) { }

    public override void Assign(IExpression value, IProgramModelExecutionContext context)
    {
        Property.SetValue(context.This().EvaluatePlain(context), value.EvaluatePlain(context));
    }
}

