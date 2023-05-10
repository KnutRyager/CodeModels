using System;
using System.Collections.Generic;
using CodeModels.AbstractCodeModels.Collection;
using CodeModels.Models;
using static CodeModels.Factory.AbstractCodeModelFactory;
using static CodeModels.Factory.CodeModelFactory;

namespace CodeModels.AbstractCodeModels.Member;

public record InstanceClass(string Identifier, NamedValueCollection Properties, List<IMethod> Methods, Namespace? Namespace = null)
    : ClassModel(Identifier, Properties, Methods, Namespace)
{
    public InstanceClass(string identifier, NamedValueCollection? properties = null, IEnumerable<IMethod>? methods = null, Namespace? @namespace = null)
    : this(identifier, NamedValues(properties), List(methods), @namespace) { }

    public override InstantiatedObject CreateInstance()
    {
        throw new NotImplementedException();
    }
}
public record InstanceClassFromReflection(Type ReflectedType) : StaticClass(ReflectedType.Name, NamedValues(ReflectedType), Methods(ReflectedType), Namespace(ReflectedType));
