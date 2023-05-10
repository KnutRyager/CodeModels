using System;
using System.Collections.Generic;
using CodeModels.AbstractCodeModels.Collection;
using CodeModels.Models;
using static CodeModels.Factory.AbstractCodeModelFactory;
using static CodeModels.Factory.CodeModelFactory;

namespace CodeModels.AbstractCodeModels.Member;

public record StaticClass(string Identifier, NamedValueCollection Properties, List<IMethod> Methods,
    Namespace? Namespace = null, Modifier TopLevelModifier = Modifier.None, Modifier MemberModifier = Modifier.None)
    : ClassModel(Identifier, Properties, Methods, Namespace, TopLevelModifier.SetModifier(Modifier.Static), MemberModifier.SetModifier(Modifier.Static))
{
    public StaticClass(string identifier, NamedValueCollection? properties = null,
        IEnumerable<IMethod>? methods = null, Namespace? @namespace = null)
    : this(identifier, NamedValues(properties), List(methods), @namespace) { }

    public override InstantiatedObject CreateInstance()
    {
        throw new NotImplementedException();
    }
}
public record StaticClassFromReflection(Type ReflectedType) : StaticClass(ReflectedType.Name, new(ReflectedType), Methods(ReflectedType), Namespace(ReflectedType));
