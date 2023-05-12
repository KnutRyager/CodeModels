using System.Reflection;
using CodeModels.AbstractCodeModels;
using CodeModels.Execution.Context;
using CodeModels.Factory;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Reflection;
using Common.Extensions;
using Microsoft.CodeAnalysis;

namespace CodeModels.Models.Primitives.Member;

public record PropertyFromField(FieldInfo Field)
    : AbstractProperty(TypeFromReflection.Create(Field), Field.Name, Field.IsLiteral ? CodeModelFactory.Literal(Field.GetValue(null)) : null,
        (Field.IsLiteral && !Field.IsInitOnly ? Modifier.Const : Modifier.None).SetFlags(Modifier.Public | Modifier.Field))
{
    public PropertyFromField(IFieldSymbol symbol) : this(SemanticReflection.GetField(symbol)) { }

    public override IExpression EvaluateAccess(ICodeModelExecutionContext context, IExpression instance)
        => CodeModelFactory.Literal(Field.GetValue(instance.EvaluatePlain(context)));
}
