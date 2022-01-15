using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Reflection;
using Models;

namespace CodeAnalyzation.DataTransformation;

[Model]
public class ClassAnalysis
{
    [Key] public int Id { get; set; }
    public string Name { get; set; } = default!;
    public Namespace Namespace { get; set; } = default!;
    public string Hash { get; private set; } = default!;
    [NotMapped] private Type? _primitiveType;
    public Type? PrimitiveType => _primitiveType ??= PrimitiveTypeSerialized == null ? null : ReflectionSerialization.DeserializeType(PrimitiveTypeSerialized);
    public string PrimitiveTypeSerialized { get; set; } = default!;
    public virtual ICollection<Operation> Operations { get; set; } = default!;
}
