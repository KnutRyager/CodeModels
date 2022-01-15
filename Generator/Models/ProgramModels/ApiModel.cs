using System.Collections.Generic;
using CodeAnalyzation.Models.ErDiagram;
//using System.ComponentModel.DataAnnotations;

namespace CodeAnalyzation.Models.ProgramModels;

public class ApiModel
{
    //[Key]
    public int Id { get; set; } = default!;
    public string Name { get; set; } = default!;

    public Clazz Clazz { get; set; } = default!;
    public virtual IList<ERDiagram> ERDiagrams { get; set; }

    public ApiModel()
    {
        ERDiagrams = new List<ERDiagram>();
    }
}
