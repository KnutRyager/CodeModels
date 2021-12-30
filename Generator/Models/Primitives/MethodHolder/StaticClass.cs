using System.Collections.Generic;
using System.Linq;

namespace CodeAnalyzation.Models
{
    public record StaticClass(string Identifier, PropertyCollection Properties, List<Method> Methods,
        List<IType> Constants, Namespace? Namespace = null)
        : ClassModel(Identifier, Properties, Methods, Constants, Namespace, IsStatic: true)
    {
        public StaticClass(string identifier, PropertyCollection? properties = null, IEnumerable<Method>? methods = null,
            IEnumerable<IType>? constants = null, Namespace? @namespace = null)
        : this(identifier, properties ?? new PropertyCollection(), methods?.ToList() ?? new List<Method>(), constants?.ToList() ?? new List<IType>(), @namespace) { }
    }
}