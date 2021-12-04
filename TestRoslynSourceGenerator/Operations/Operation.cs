using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Common.Reflection;
using Common.Util;
using Models;

namespace TheEverythingAPI.DataTransformation;

[Model]
public class Operation
{
    [Key] public int Id { get; set; }
    public int OutputId { get; set; }
    public int? OperationPipelineId { get; set; }
    public int? NamespaceId { get; set; }
    public string Hash { get; private set; } = default!;
    public OperationType OperationType { get; set; } = default!;
    public string Name { get; set; } = default!;
    public virtual ICollection<DataTypeOperationInput> DataTypeOperationInput { get; set; } = default!;
    public virtual ICollection<DataTypeOperationGenericParameter> GenericParameterRelations { get; set; } = default!;
    public DataType Output { get; set; } = default!;
    public OperationPipeline? OperationPipeline { get; set; }
    public Namespace? Namespace { get; set; }
    public string FuncReference { get; set; } = default!;
    [NotMapped] public IList<DataType>? _genericParameters;
    [NotMapped] public IList<DataType> GenericParameters => _genericParameters ??= CollectionUtil.OnlyIfNoNulls(GenericParameterRelations?.OrderBy(x => x.ParameterIndex).Select(x => x.DataType))?.ToArray()!;
    [NotMapped] private IList<DataType>? _inputs;
    [NotMapped] public IList<DataType> Inputs => _inputs ??= CollectionUtil.OnlyIfNoNulls(DataTypeOperationInput?.OrderBy(x => x.ParameterIndex).Select(x => x.DataType))?.ToArray()!;
    [NotMapped] public IList<DataType> AllDataTypes => Inputs.ToList().Concat(new[] { Output }).Distinct().ToArray();
    [NotMapped] public DataType? Input => Inputs.FirstOrDefault();
    [NotMapped] public MethodInfo? _method;
    [NotMapped] public MethodInfo? Method => _method ?? DeserilizeMember() as MethodInfo;
    [NotMapped] public MethodInfo? MethodGeneric { get; set; }
    [NotMapped] public PropertyInfo? _property;
    [NotMapped] public PropertyInfo? Property => _property ?? DeserilizeMember() as PropertyInfo;
    [NotMapped] public FieldInfo? _field;
    [NotMapped] public FieldInfo? Field => _field ?? DeserilizeMember() as FieldInfo;
    [NotMapped] private ConstructorInfo? _constructor;
    [NotMapped] public ConstructorInfo? Constructor => _constructor ?? DeserilizeMember() as ConstructorInfo;
    public MemberInfo? MemberInfo => Method ?? Property ?? (MemberInfo?)Field ?? Constructor;
    public Type? DeclaringType => Method?.DeclaringType ?? Property?.DeclaringType ?? Field?.DeclaringType;
    public int? InputId => Inputs.FirstOrDefault()?.Id;
    [NotMapped] private int? _inputCount;
    public int InputCount => _inputCount ??= Inputs.Where(x => x.PrimitiveType != typeof(void)).Count();
    public Assembly? Assembly => Namespace?.Assembly;
    public bool IsEnum { get; set; }
    public bool IsLiteral { get; set; }

    public Operation()
    {
        Init();
    }

    public Operation(IEnumerable<DataType> inputs, DataType output, OperationType operationType = OperationType.None, Namespace? @namespace = null, IEnumerable<DataType>? genericParameters = null, MethodInfo? method = null, PropertyInfo? property = null, FieldInfo? field = null, ConstructorInfo? constructorInfo = null)
    {
        _inputs = inputs.ToArray();
        _genericParameters = genericParameters?.ToArray() ?? Array.Empty<DataType>();
        DataTypeOperationInput = new List<DataTypeOperationInput>(inputs.Select((x, index) => new DataTypeOperationInput() { DataType = x, ParameterIndex = index }));
        GenericParameterRelations = new List<DataTypeOperationGenericParameter>(genericParameters?.Select((x, index) => new DataTypeOperationGenericParameter(this, x, index)) ?? Enumerable.Empty<DataTypeOperationGenericParameter>());
        Output = output;
        MethodGeneric = method;
        _constructor = constructorInfo;
        _method = method == null || genericParameters == null || !genericParameters.Any() ? method : method.MakeGenericMethod(genericParameters.Select(x => x.PrimitiveType!).ToArray());   // TODO: Primitive type gets real type?
        _property = property;
        OperationType = operationType != OperationType.None ? operationType : method != null ? OperationType.Method : property != null ? OperationType.Property : field != null ? OperationType.Field : OperationType.None;
        _field = field;
        Namespace = @namespace;
        Init();
    }

    public Operation(DataType? input, DataType output, OperationType operationType = OperationType.None, Namespace? @namespace = null, IEnumerable<DataType>? genericParameters = null, MethodInfo? method = null, PropertyInfo? property = null, FieldInfo? field = null, ConstructorInfo? constructorInfo = null)
        : this(input == null ? Array.Empty<DataType>() : new[] { input }, output, operationType, @namespace, genericParameters, method, property, field, constructorInfo) { }

    public static Operation MakeMethod(IEnumerable<DataType> inputs, DataType output, MethodInfo method, IEnumerable<DataType>? genericParameters = null, Namespace? @namespace = null)
        => new(inputs, output, OperationType.Method, @namespace, genericParameters, method: method);

    public static Operation MakeMethod(DataType input, DataType output, MethodInfo method, IEnumerable<DataType>? genericParameters = null, Namespace? @namespace = null)
        => MakeMethod(new[] { input }, output, method, genericParameters, @namespace);

    public static Operation MakeMethod(DataType output, MethodInfo method, IEnumerable<DataType>? genericParameters = null, Namespace? @namespace = null)
        => MakeMethod(Array.Empty<DataType>(), output, method, genericParameters, @namespace);

    public static Operation MakeProperty(DataType input, DataType output, PropertyInfo property, IEnumerable<DataType>? genericParameters = null, Namespace? @namespace = null)
        => new(input, output, OperationType.Property, @namespace, genericParameters, property: property);

    public static Operation MakeField(DataType input, DataType output, FieldInfo field, IEnumerable<DataType>? genericParameters = null, Namespace? @namespace = null)
        => new(input, output, OperationType.Field, @namespace, genericParameters, field: field);

    public static Operation Conztructor(DataType target, ConstructorInfo? constructorInfo, Namespace? @namespace = null, params DataType[] inputs)
        => new(inputs, target, OperationType.Constructor, @namespace, constructorInfo: constructorInfo);

    public static Operation Inheritance(DataType child, DataType parent, Namespace? @namespace = null)
        => new(child, parent, OperationType.Inheritance, @namespace);

    public static Operation Implementation(DataType child, DataType parent, Namespace? @namespace = null)
        => new(child, parent, OperationType.Implementation, @namespace);

    public static Operation TypeCast(DataType source, DataType target, Namespace? @namespace = null)
        => new(source, target, OperationType.Cast, @namespace);

    public static Operation Identity(DataType source, Namespace? @namespace = null)
        => new(source, source, OperationType.Identity, @namespace);

    public static Operation Provider(DataType source, Namespace? @namespace = null)
        => new(Array.Empty<DataType>(), source, OperationType.Provider, @namespace);

    public static Operation Pipeline(OperationPipeline pipeline, Namespace? @namespace = null)
        => new(pipeline.Inputs, pipeline.Output, OperationType.Pipeline, @namespace) { Name = pipeline.Name, OperationPipeline = pipeline };

    public static Operation UnaryOperator(DataType type, OperationType operationType, Namespace? @namespace = null)
        => UnaryOperator(type, type, operationType, @namespace);

    public static Operation UnaryOperator(DataType input, DataType output, OperationType operationType, Namespace? @namespace = null)
        => operationType.IsUnaryOperator() ? new(input, output, operationType, @namespace) : throw new ArgumentException($"Not a unary operator: '{operationType}'");

    public static Operation BinaryOperator(DataType type, OperationType operationType, Namespace? @namespace = null)
        => operationType.IsBinaryOperator() ? new(new[] { type, type }, type, operationType, @namespace) : throw new ArgumentException($"Not a binary operator: '{operationType}'");

    public static Operation TernaryOperator(DataType type, DataType output, OperationType operationType = OperationType.Ternary, Namespace? @namespace = null)
        => operationType.IsTernaryOperator() ? new(type, output, operationType, @namespace) : throw new ArgumentException($"Not a ternary operator: '{operationType}'");

    public static Operation AnyArgOperator(IEnumerable<DataType> inputs, DataType output, OperationType operationType, Namespace? @namespace = null)
        => operationType.IsAnyArgOperator() ? new(inputs, output, operationType, @namespace) : throw new ArgumentException($"Not an any arg operator: '{operationType}'");

    public void Init()
    {
        if (OperationType == OperationType.Identity) return;
        if (FuncReference == null)
        {
            FuncReference = Method != null ? ReflectionSerialization.SerializeMethod(Method)
                : Property != null ? ReflectionSerialization.SerializeProperty(Property)
                : Field != null ? ReflectionSerialization.SerializeField(Field)
                : OperationType.ToString();
            IsEnum = Field?.DeclaringType?.IsEnum ?? false;
            IsLiteral = Field?.IsLiteral ?? false;
        }
    }

    private MemberInfo? DeserilizeMember()
    {
        if (FuncReference != null)
        {
            if (_method == null && OperationType == OperationType.Method) _method = ReflectionSerialization.DeserializeMethod(FuncReference);
            if (_property == null && OperationType == OperationType.Property) _property = ReflectionSerialization.DeserializeProperty(FuncReference);
            if (_field == null && OperationType == OperationType.Field) _field = ReflectionSerialization.DeserializeField(FuncReference);
        }
        return _method ?? _property ?? (MemberInfo?)_field ?? _constructor;
    }
}
