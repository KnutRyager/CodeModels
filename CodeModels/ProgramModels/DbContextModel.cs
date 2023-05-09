//using System.ComponentModel.DataAnnotations;

using System.Collections.Generic;
using CodeModels.ProgramModels.ErDiagram;

namespace CodeModels.ProgramModels;

public record DbContextModel
{
    //[Key]
    public int Id { get; set; } = default!;
    public string Name { get; set; } = default!;

    public Clazz Clazz { get; set; } = default!;
    public virtual IList<ERDiagram> ERDiagrams { get; set; }

    public DbContextModel()
    {
        ERDiagrams = new List<ERDiagram>();
    }
}
