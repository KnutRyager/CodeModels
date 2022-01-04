#nullable disable
using System.ComponentModel.DataAnnotations;
using Models;

namespace CodeAnalyzation.DataTransformation
{
    [Model]
    public class DataTypeOperationInput
    {
        [Key] public int Id { get; set; }
        public int ParameterIndex { get; set; }
        public int DataTypeId { get; set; }
        public int OperationId { get; set; }

        public DataType DataType { get; set; }
        public Operation Operation { get; set; }

        public override string ToString() => $"{Operation}({ParameterIndex}.{DataType})";
    }
}