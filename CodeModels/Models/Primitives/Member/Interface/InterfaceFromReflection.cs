using System;
using System.Linq;
using Common.Reflection;
using CodeModels.AbstractCodeModels.Collection;
using CodeModels.Factory;
using CodeModels.AbstractCodeModels.Member;

namespace CodeModels.Models;

public record InterfaceFromReflection(Type ReflectedType)
    : InterfaceModel(ReflectedType.Name,
new NamedValueCollection(ReflectedType.GetProperties(), ReflectedType.GetFields()),
ReflectedType.GetMethods().Select(x => CodeModelsFromReflection.Method(x)).ToList<IMethod>(),
        new Namespace(ReflectedType.Namespace),
    ReflectionUtil.IsStatic(ReflectedType));
