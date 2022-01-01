using System;
using System.Collections.Generic;
using System.Linq;
using static CodeAnalyzation.Models.CodeModelFactory;

namespace CodeAnalyzation.Models
{
    public record StaticClass(string Identifier, PropertyCollection Properties, List<IMethod> Methods,
        Namespace? Namespace = null)
        : ClassModel(Identifier, Properties, Methods, Namespace, IsStatic: true)
    {
        public StaticClass(string identifier, PropertyCollection? properties = null,
            IEnumerable<IMethod>? methods = null, Namespace? @namespace = null)
        : this(identifier, PropertyCollection(properties), List(methods), @namespace) { }
    }
    public record StaticClassFromReflection(Type Type) : StaticClass(Type.Name, new(Type), Methods(Type), Namespace(Type));
}