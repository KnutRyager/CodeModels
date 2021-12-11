//using System.ComponentModel.DataAnnotations;

using System.Collections.Generic;

namespace TheEverythingAPI.Modelling.ProgramModel
{
    public class ControllerModel
    {
        //[Key]
        public int Id { get; set; } = default!;
        public string Name { get; set; } = default!;

        public Clazz Clazz { get; set; } = default!;
        public virtual IList<ERDiagram> ERDiagrams { get; set; }

        public ControllerModel()
        {
            ERDiagrams = new List<ERDiagram>();
        }
    }
}