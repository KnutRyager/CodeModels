using System;
using System.Collections.Generic;
using System.Linq;
using static CodeAnalyzation.Models.CodeModelFactory;

namespace CodeAnalyzation.Models;

public record InstanceClass(string Identifier, PropertyCollection Properties, List<IMethod> Methods, Namespace? Namespace = null)
    : ClassModel(Identifier, Properties, Methods, Namespace)
{
    public InstanceClass(string identifier, PropertyCollection? properties = null, IEnumerable<IMethod>? methods = null, Namespace? @namespace = null)
    : this(identifier, PropertyCollection(properties), List(methods), @namespace) { }
}
public record InstanceClassFromReflection(Type ReflectedType) : StaticClass(ReflectedType.Name, PropertyCollection(ReflectedType), Methods(ReflectedType), Namespace(ReflectedType));
