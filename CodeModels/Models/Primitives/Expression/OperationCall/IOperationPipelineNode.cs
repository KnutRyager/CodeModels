using System.Collections.Generic;

namespace CodeModels.Models;

public interface IOperationPipelineNode
{
    int Index { get; set; }
    IList<IOperationPipelineNode> InputNodes { get; }
    IOperationPipelineNode? Output { get; }
    IOperationPipeline OperationPipeline { get; }
    int ParameterIndex { get; }
    IOperation Operation { get; }
    IList<IOperation> Inputs { get; }
    IList<IOperationPipelineNode> AllNodes { get; }
    int OperationInputCount { get; }
    int OperationSelfInputCount { get; }
    OperationType OperationType { get; }
    int GetLeafInputCount();
}

public interface IOperationPipelineNode<TOperationPipeline, TOperationPipelineNode, TOperation> : IOperationPipelineNode
    where TOperationPipeline : IOperationPipeline
    where TOperationPipelineNode : IOperationPipelineNode
    where TOperation : IOperation
{
    new IList<TOperationPipelineNode> InputNodes { get; }
    new TOperationPipelineNode? Output { get; }
    new TOperationPipeline OperationPipeline { get; }
    new TOperation Operation { get; }
    new IList<TOperation> Inputs { get; }
    new IList<TOperationPipelineNode> AllNodes { get; }
}
