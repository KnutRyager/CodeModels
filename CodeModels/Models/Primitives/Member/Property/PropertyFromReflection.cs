using System.Collections.Generic;
using System.Reflection;
using CodeModels.AbstractCodeModels.Member;
using CodeModels.Execution.Context;
using CodeModels.Execution.Scope;
using CodeModels.Factory;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Reflection;
using Common.Extensions;
using Common.Reflection;
using Microsoft.CodeAnalysis;

namespace CodeModels.Models;

public record PropertyFromReflection(PropertyInfo Property)
    : AbstractProperty(TypeFromReflection.Create(Property), Property.Name, ReflectionUtil.IsStatic(Property) ? CodeModelFactory.Literal(Property.GetValue(null)) : null,
        (!Property.CanWrite ? Modifier.Readonly : Modifier.None).SetFlags(Modifier.Public | Modifier.Property))
{
    public static PropertyFromReflection Create(PropertyInfo property) => new(property);
    
    public PropertyFromReflection(IPropertySymbol symbol) : this(SemanticReflection.GetProperty(symbol)) { }

    public override void Assign(IExpression value, ICodeModelExecutionContext context, IList<ICodeModelExecutionScope> scopes)
    {
        try
        {
            context.EnterScopes(scopes);
            Property.SetValue(context.This().EvaluatePlain(context), value.EvaluatePlain(context));
        }
        finally
        {
            context.ExitScopes(scopes);
        }
    }
}

