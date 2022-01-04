using System;
using System.Collections.Generic;

namespace CodeAnalyzation.Models
{
    public interface IDataType
    {
        string Name { get; }
        Type? PrimitiveType { get; }
        string? NamespaceStr { get; }
        // SpecialType? SpecialType { get; }
        IDataType? UnderlyingType { get; }
        IOperation? Lambda { get; }
        INamespace? Namespace { get; }
        ICollection<IOperationPipeline> PipelinesIn { get; }
        ICollection<IOperationPipeline> PipelinesOut { get; }
        //ICollection<IDataTypeOperationInput> DataTypeOperationInput { get; }
        ICollection<IOperation> OperationsOut { get; }
        ICollection<IDataType> Inheritors { get; }
        //ICollection<IDataTypeOperationGenericParameter> GenericOperationParameterRelations { get; }
        ICollection<IOperation> OperationsIn { get; }
        //ICollection<IDataTypeDataTypeGenericParameter> GenericDataTypeParameterChildRelations { get; }
        //ICollection<IDataTypeDataTypeGenericParameter> GenericDataTypeParameterParentRelations { get; }
        IList<IDataType> GenericParameters { get; }
        IAssembly? Assembly { get; }
        bool IsGeneric { get; }
        IType Type { get; }
    }
    public interface IDataType<TAssembly, TNamespace, TOperationPipeline, TOperation, TDataType> : IDataType
        where TAssembly : IAssembly
        where TNamespace : INamespace
        where TDataType : IDataType
        where TOperationPipeline : IOperationPipeline
        where TOperation : IOperation
    {
        new TDataType? UnderlyingType { get; }
        new TOperation? Lambda { get; }
        new TNamespace? Namespace { get; }
        new ICollection<TOperationPipeline> PipelinesIn { get; }
        new ICollection<TOperationPipeline> PipelinesOut { get; }
        //ICollection<IDataTypeOperationInput> DataTypeOperationInput { get; }
        new ICollection<TOperation> OperationsOut { get; }
        new ICollection<TDataType> Inheritors { get; }
        //ICollection<IDataTypeOperationGenericParameter> GenericOperationParameterRelations { get; }
        new ICollection<TOperation> OperationsIn { get; }
        //ICollection<IDataTypeDataTypeGenericParameter> GenericDataTypeParameterChildRelations { get; }
        //ICollection<IDataTypeDataTypeGenericParameter> GenericDataTypeParameterParentRelations { get; }
        new IList<TDataType> GenericParameters { get; }
        new TAssembly? Assembly { get; }
    }
}