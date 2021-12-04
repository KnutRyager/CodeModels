#nullable disable
using System.ComponentModel.DataAnnotations;
using Models;

namespace TheEverythingAPI.DataTransformation;

[Model]
public class OperationPipelineOperationInput
{
    [Key] public int Id { get; set; }
    public int ParameterIndex { get; set; }
    public int OperationPipelineNodeId { get; set; }
    public int OperationId { get; set; }

    public OperationPipelineNode OperationPipelineNode { get; set; }
    public Operation Operation { get; set; }
}
