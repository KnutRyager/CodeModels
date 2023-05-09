﻿using System.Reflection;
using CodeModels.Execution.Context;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Reflection;
using Common.Extensions;
using Microsoft.CodeAnalysis;

namespace CodeModels.Models;

public record PropertyFromField(FieldInfo Field)
    : Property(new TypeFromReflection(Field.FieldType), Field.Name, Field.IsLiteral ? new LiteralExpression(Field.GetValue(null)) : null,
        (Field.IsLiteral && !Field.IsInitOnly ? Modifier.Const : Modifier.None).SetFlags(Modifier.Public | Modifier.Field))
{
    public PropertyFromField(IFieldSymbol symbol) : this(SemanticReflection.GetField(symbol)) { }

    public override IExpression EvaluateAccess(ICodeModelExecutionContext context, IExpression instance)
        => new LiteralExpression(Field.GetValue(instance.EvaluatePlain(context)));
}
