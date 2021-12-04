using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Reflection;
using Common.Util;
using Models;

namespace TheEverythingAPI.DataTransformation;

[Model]
public class Namespace
{
    [Key] public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public int Depth { get; set; }
    public bool IsGenericTypeDefinition { get; set; }
    public bool IsConstructedGenericType { get; set; }
    public int? ParentNamespaceId { get; set; }
    public int? GenericDefinitionNamespaceId { get; set; }
    public int? AssemblyId { get; set; }
    public Namespace? ParentNamespace { get; set; }
    public Namespace? GenericDefinitionNamespace { get; set; }
    public Assembly? Assembly { get; set; }
    public virtual ICollection<Namespace> ChildrenNamespaces { get; set; } = default!;
    public virtual ICollection<Namespace> GenericInstanceNamespaces { get; set; } = default!;
    public virtual ICollection<NamespaceContainment> AllParentNamespaces { get; set; } = default!;
    public virtual ICollection<NamespaceContainment> AllChildNamespaces { get; set; } = default!;
    public virtual ICollection<DataType> DataTypes { get; set; } = default!;
    public virtual ICollection<Operation> Operations { get; set; } = default!;
    public string? PrimitiveTypeSerialized { get; set; }
    [NotMapped] private Type? _type;
    public Type? Type => _type ??= _type ??= PrimitiveTypeSerialized == null ? null : ReflectionSerialization.DeserializeType(PrimitiveTypeSerialized);
    public bool IsType => Type != null;
    [NotMapped] private IList<Namespace>? _parentNamespaces;
    [NotMapped] public IList<Namespace> ParentNamespaces => _parentNamespaces ??= CollectionUtil.OnlyIfNoNulls(AllParentNamespaces?.Select(x => x.ParentNamespace))?.ToArray()!;
    [NotMapped] private IList<Namespace>? _childNamespaces;
    [NotMapped] public IList<Namespace> ChildNamespaces => _childNamespaces ??= CollectionUtil.OnlyIfNoNulls(AllChildNamespaces?.Select(x => x.ChildNamespace))?.ToArray()!;
    [NotMapped] private IList<Namespace>? _directChildNamespaces;
    [NotMapped] public IList<Namespace> DirectChildNamespaces => _directChildNamespaces ??= CollectionUtil.OnlyIfNoNulls(AllChildNamespaces?.Select(x => x.ChildNamespace))?.Where(x => x.Depth == Depth + 1).ToArray()!;
    public string Hash { get; private set; } = default!;

    public Namespace() { }

    public Namespace(string name, Namespace? parentNamespace = null, Assembly? assembly = null, Type? type = null, Namespace? genericDefinitionNamespace = null)
    {
        ChildrenNamespaces = new List<Namespace>();
        GenericInstanceNamespaces = new List<Namespace>();
        AllParentNamespaces = new List<NamespaceContainment>();
        AllChildNamespaces = new List<NamespaceContainment>();
        DataTypes = new List<DataType>();
        Operations = new List<Operation>();
        _type = type;
        PrimitiveTypeSerialized = _type != null ? ReflectionSerialization.SerializeType(_type) : null;
        Name = name.Split(".").Last();
        ParentNamespace = parentNamespace;
        GenericDefinitionNamespace = genericDefinitionNamespace;
        FullName = $"{ParentNamespace?.FullName}{(ParentNamespace == null ? "" : ".")}{(type != null ? FormatUtil.Format(type) : Name)}";
        IsGenericTypeDefinition = Type?.IsGenericType ?? false;
        IsConstructedGenericType = Type?.IsConstructedGenericType ?? false;
        Assembly = assembly ?? parentNamespace?.Assembly;
        var nextParent = parentNamespace;
        while (nextParent != null)
        {
            AllParentNamespaces.Add(new(nextParent, this));
            nextParent = nextParent.ParentNamespace;
        }
        Hash = CryptoUtil.HashStr(FullName, PrimitiveTypeSerialized, Assembly?.Hash);
        Depth = FullName.Count(x => x == '.');
    }

    public Namespace(Type type, Namespace? parentNamespace = null, Assembly? assembly = null) : this(type.Name, parentNamespace, assembly, type) { }

    public override string ToString() => Name;
}
