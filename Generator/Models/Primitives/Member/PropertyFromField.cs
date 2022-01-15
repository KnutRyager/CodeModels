using System.Reflection;
using Common.Util;

namespace CodeAnalyzation.Models;

public record PropertyFromField(FieldInfo Field)
    : Property(new TypeFromReflection(Field.FieldType), Field.Name, Field.IsLiteral ? new LiteralExpression(Field.GetValue(null)) : null,
        (Field.IsLiteral && !Field.IsInitOnly ? Modifier.Const : Modifier.None).SetFlags(Modifier.Public | Modifier.Field));
