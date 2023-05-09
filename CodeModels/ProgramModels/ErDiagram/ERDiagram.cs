#nullable disable
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

using System.Collections.Generic;

namespace CodeModels.ProgramModels.ErDiagram;

public class ERDiagram
{
    //[Key]
    public int Id { get; set; }
    public string Name { get; set; }

    //[NotMapped]
    public virtual IList<Clazz> Clazzes { get; set; }
    public virtual IList<ClazzERDiagram> ClazzERDiagrams { get; set; }

    public ERDiagram()
    {
        Clazzes = new List<Clazz>();
        ClazzERDiagrams = new List<ClazzERDiagram>();
    }
}
