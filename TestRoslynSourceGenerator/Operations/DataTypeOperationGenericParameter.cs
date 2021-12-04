using System.ComponentModel.DataAnnotations;
using Models;

namespace TheEverythingAPI.DataTransformation;

[Model]
public class DataTypeOperationGenericParameter
{
    [Key] public int Id { get; set; }
    public int ParameterIndex { get; set; }
    public int DataTypeId { get; set; }
    public int OperationId { get; set; }

    public DataType DataType { get; set; } = default!;
    public Operation Operation { get; set; } = default!;

    public DataTypeOperationGenericParameter() { }

    public DataTypeOperationGenericParameter(Operation operation, DataType datatype, int parameterIndex)
    {
        Operation = operation;
        DataType = datatype;
        ParameterIndex = parameterIndex;
    }

    public override string ToString() => $"{Operation}({ParameterIndex}.{DataType})";
}
