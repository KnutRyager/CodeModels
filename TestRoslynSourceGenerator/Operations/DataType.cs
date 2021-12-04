using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Reflection;
using Common.Util;
using Models;

namespace TheEverythingAPI.DataTransformation;

[Model]
public class DataType
{
    [Key] public int Id { get; set; }
    public string Name { get; set; } = default!;
    public int? UnderlyingTypeId { get; set; }
    public int? LambdaId { get; set; }
    public int? NamespaceId { get; set; }
    public string Hash { get; private set; } = default!;
    [NotMapped] private Type? _primitiveType;
    public Type? PrimitiveType => _primitiveType ??= PrimitiveTypeSerialized == null ? null : ReflectionSerialization.DeserializeType(PrimitiveTypeSerialized);
    public string? NamespaceStr => Namespace?.Name ?? null;
    public string PrimitiveTypeSerialized { get; set; } = default!;
    public SpecialType? SpecialType { get; set; }
    public DataType? UnderlyingType { get; set; }
    public Operation? Lambda { get; set; }
    public Namespace? Namespace { get; set; }
    public virtual ICollection<OperationPipeline> PipelinesIn { get; set; } = default!;
    public virtual ICollection<OperationPipeline> PipelinesOut { get; set; } = default!;
    public virtual ICollection<DataTypeOperationInput> DataTypeOperationInput { get; set; } = default!;
    public virtual ICollection<Operation> OperationsOut { get; set; } = default!;
    public virtual ICollection<DataType> Inheritors { get; set; } = default!;
    public virtual ICollection<DataTypeOperationGenericParameter> GenericOperationParameterRelations { get; set; } = default!;
    [NotMapped] public virtual ICollection<Operation> OperationsIn { get; set; } = default!;
    public virtual ICollection<DataTypeDataTypeGenericParameter> GenericDataTypeParameterChildRelations { get; set; } = default!;
    public virtual ICollection<DataTypeDataTypeGenericParameter> GenericDataTypeParameterParentRelations { get; set; } = default!;
    [NotMapped] public IList<DataType>? _genericParameters;
    [NotMapped] public IList<DataType> GenericParameters => _genericParameters ??= CollectionUtil.OnlyIfNoNulls(GenericDataTypeParameterChildRelations?.Select(x => x.Child))?.ToArray()!;
    public Assembly? Assembly => Namespace?.Assembly;
    public bool IsGeneric => GenericDataTypeParameterChildRelations.Count > 0;
}
