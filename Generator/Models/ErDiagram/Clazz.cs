#nullable disable
using System.Collections.Generic;

namespace CodeAnalyzation.Models.ErDiagram
{
    public class Clazz
    {
        public int Id { get; set; }
        public int DataTypeId { get; set; }
        public string Name { get; set; }

        public virtual IList<Field> Fields { get; set; }
        public virtual IList<ERDiagram> ERDiagrams { get; set; }
        public virtual IList<ClazzERDiagram> ClazzERDiagrams { get; set; }

        public Clazz()
        {
            Fields = new List<Field>();
            ERDiagrams = new List<ERDiagram>();
            ClazzERDiagrams = new List<ClazzERDiagram>();
        }
    }
}