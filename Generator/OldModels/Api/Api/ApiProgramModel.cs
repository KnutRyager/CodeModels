using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;

namespace TheEverythingAPI.Modelling.ProgramModel
{
    public class ApiProgramModel
    {
        //[Key]
        public int Id { get; set; } = default!;
        public string Name { get; set; } = default!;

        public virtual ERDiagram ERDiagram { get; set; } = default!;
        public virtual IList<ApiModel> ApiModels { get; set; }
        public virtual IList<ControllerModel> ControllerModels { get; set; }

        public ApiProgramModel()
        {
            ApiModels = new List<ApiModel>();
            ControllerModels = new List<ControllerModel>();
        }
    }
}