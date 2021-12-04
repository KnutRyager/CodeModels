using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Util;
using Models;

namespace TheEverythingAPI.DataTransformation;

[Model]
public class OperationPipelineNode
{
    [Key] public int Id { get; set; }
    public int Index { get; set; }
    public int? OutputId { get; set; }
    public int OperationId { get; set; }
    public int OperationPipelineId { get; set; }

    public IList<OperationPipelineNode> InputNodes { get; set; } = default!;
    public OperationPipelineNode? Output { get; set; }
    public OperationPipeline OperationPipeline { get; set; } = default!;
    public int ParameterIndex { get; set; }
    public Operation Operation { get; set; } = default!;
    public string Hash { get; private set; } = default!;
    [NotMapped] public IList<Operation> Inputs { get; } = default!;
    [NotMapped] private IList<OperationPipelineNode> _allNodes = default!;
    [NotMapped] public IList<OperationPipelineNode> AllNodes => _allNodes ??= InputNodes.SelectMany(x => x.AllNodes).ToList().Concat(new[] { this }).ToList();
    public int OperationInputCount => Operation.InputCount;
    public int OperationSelfInputCount => OperationInputCount - Inputs.Count;
    public OperationType OperationType => Operation.OperationType;

    public List<DataType> GetLeafInputTypes()
    {
        if (OperationType == OperationType.Provider) return new() { Operation.Output };
        var result = InputNodes.SelectMany(x => x.GetLeafInputTypes()).ToList();
        var missingInput = Operation.InputCount - Inputs.Count;
        for (var i = Inputs.Count; i < Inputs.Count + missingInput; i++)
        {
            result.Add(Operation.Inputs[i]);
        }
        return result;
    }

    public List<Operation> GetLeafInputs()
    {
        if (OperationType == OperationType.Provider || InputNodes.Count == 0) return new() { Operation };
        var result = InputNodes.SelectMany(x => x.GetLeafInputs()).ToList();
        var missingInput = Operation.InputCount - Inputs.Count;
        for (var i = Inputs.Count; i < Inputs.Count + missingInput; i++)
        {
            result.Add(Operation.Provider(Operation.Inputs[i]));
        }
        return result;
    }
}
