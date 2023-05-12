#nullable disable
//using System.ComponentModel.DataAnnotations;

namespace CodeModels.ProgramModels.ErDiagram;

public class ForeignKey
{
    //[Key]
    public int Id { get; set; }
    public int FieldIdFrom { get; set; }
    public int FieldIdTo { get; set; }
    public string Name { get; set; }

    public Field FieldFrom { get; set; }
    public Field FieldTo { get; set; }

}
