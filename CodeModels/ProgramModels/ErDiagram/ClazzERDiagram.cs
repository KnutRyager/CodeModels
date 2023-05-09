#nullable disable

using CodeModels;

namespace CodeModels.ProgramModels.ErDiagram;

public class ClazzERDiagram
{
    public int ERDiagramId { get; set; }
    public int ClazzId { get; set; }

    public ERDiagram ERDiagram { get; set; }
    public Clazz Clazz { get; set; }

    public ClazzERDiagram()
    {
    }
}
