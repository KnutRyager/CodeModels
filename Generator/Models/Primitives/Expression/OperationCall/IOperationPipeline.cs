using System.Collections.Generic;

namespace CodeAnalyzation.Models
{
    public interface IOperationPipeline
    {
        string Name { get; }

        IList<IOperationPipelineNode> OperationPipelineNodes { get; }
        IOperationPipelineNode OutputNode { get; }
        IList<IDataType> Inputs { get; }
        IList<IDataType> InputTypes { get; }
        IDataType Output { get; }
        IList<IOperation> Operations { get; }
        IType Type { get; }
    }

    public interface IOperationPipeline<TOperationPipelineNode, TOperation, TDataType> : IOperationPipeline
    where TOperationPipelineNode : IOperationPipelineNode
        where TDataType : IDataType
        where TOperation : IOperation
    {

        new IList<TOperationPipelineNode> OperationPipelineNodes { get; }
        new TOperationPipelineNode OutputNode { get; }
        new IList<TDataType> Inputs { get; }
        new IList<TDataType> InputTypes { get; }
        new TDataType Output { get; }
        new IList<TOperation> Operations { get; }
    }
}