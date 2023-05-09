using System.Collections.Generic;
using CodeAnalyzation.DataTransformation;

namespace CodeAnalyzation.Models;

public interface IOperation
{
    OperationType OperationType { get; }
    string Name { get; }
    // ICollection<DataTypeOperationInput> DataTypeOperationInput { get; set; } = default!;
    // ICollection<DataTypeOperationGenericParameter> GenericParameterRelations { get; set; } = default!;
    IDataType Output { get; }
    IOperationPipeline? OperationPipeline { get; }
    INamespace? Namespace { get; }
    string FuncReference { get; }
    bool IsEnum { get; }
    bool IsLiteral { get; }
    bool IsStatic { get; }
}

public interface IOperation<TNamespace, TDataType, TOperationPipeline> : IOperation
    where TNamespace : INamespace
    where TDataType : IDataType
    where TOperationPipeline : IOperationPipeline
{
    new TNamespace? Namespace { get; }
    new TDataType Output { get; }
    new TOperationPipeline? OperationPipeline { get; }
}
