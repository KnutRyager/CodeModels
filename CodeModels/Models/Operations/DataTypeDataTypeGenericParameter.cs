using System.ComponentModel.DataAnnotations;
using Common.Util;
using Models;

namespace CodeAnalyzation.DataTransformation;

[Model]
public class DataTypeDataTypeGenericParameter
{
    [Key] public int Id { get; set; }
    public int ParameterIndex { get; set; }
    public int ParentId { get; set; }
    public int ChildId { get; set; }

    public DataType Parent { get; set; } = default!;
    public DataType Child { get; set; } = default!;
    public string Hash { get; private set; } = default!;

    public DataTypeDataTypeGenericParameter() { }

    public DataTypeDataTypeGenericParameter(DataType parent, DataType child, int parameterIndex)
    {
        Parent = parent;
        Child = child;
        ParameterIndex = parameterIndex;
        Hash = CryptoUtil.HashStr(child.Hash, parameterIndex);
    }

    public override string ToString() => $"{Parent}({ParameterIndex}.{Child})";
}
