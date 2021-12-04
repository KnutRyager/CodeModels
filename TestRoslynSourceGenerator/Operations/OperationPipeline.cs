using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Util;
using Models;

namespace TheEverythingAPI.DataTransformation;

[Model]
public class OperationPipeline
{
    [Key] public int Id { get; set; }
    public string Name { get; set; } = default!;

    public virtual IList<OperationPipelineNode> OperationPipelineNodes { get; } = default!;
    public string Hash { get; private set; } = default!;
    [NotMapped] private OperationPipelineNode? _outputNode;
    [NotMapped] public OperationPipelineNode OutputNode => _outputNode ??= OperationPipelineNodes.First(x => x.Output == null);
    [NotMapped] private IList<DataType>? _inputs;
    [NotMapped] public IList<DataType> Inputs => _inputs ??= CollectionUtil.OnlyIfNoNulls(OutputNode.GetLeafInputs().SelectMany(x => x.OperationType == OperationType.Provider ? new[] { x.Output } : x.Inputs).Where(x => x.PrimitiveType != typeof(void)))?.ToArray()!;
    [NotMapped] private IList<DataType>? _inputTypes;
    [NotMapped] public IList<DataType> InputTypes => _inputTypes ??= OutputNode.GetLeafInputTypes().Where(x => x.PrimitiveType != typeof(void)).ToArray();
    [NotMapped] private DataType? _output;
    [NotMapped] public int OutputId => OutputNode.Operation.OutputId;
    [NotMapped] public DataType Output => _output ??= OutputNode.Operation.Output;
    [NotMapped] private IList<Operation>? _operations;
    [NotMapped] public IList<Operation> Operations => _operations ??= CollectionUtil.OnlyIfNoNulls(OperationPipelineNodes?.Select(x => x.Operation))?.ToArray()!;
}
