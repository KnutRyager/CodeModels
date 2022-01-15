using System;
using System.Collections.Generic;
using System.Linq;
using Common.Util;
using static CodeAnalyzation.Models.CodeModelFactory;

namespace CodeAnalyzation.Models
{
    public record StaticClass(string Identifier, PropertyCollection Properties, List<IMethod> Methods,
        Namespace? Namespace = null, Modifier TopLevelModifier = Modifier.None, Modifier MemberModifier = Modifier.None)
        : ClassModel(Identifier, Properties, Methods, Namespace, TopLevelModifier.SetModifier(Modifier.Static), MemberModifier.SetModifier(Modifier.Static))
    {
        public StaticClass(string identifier, PropertyCollection? properties = null,
            IEnumerable<IMethod>? methods = null, Namespace? @namespace = null)
        : this(identifier, PropertyCollection(properties), List(methods), @namespace) { }
    }
    public record StaticClassFromReflection(Type ReflectedType) : StaticClass(ReflectedType.Name, new(ReflectedType), Methods(ReflectedType), Namespace(ReflectedType));
}