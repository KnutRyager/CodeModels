using System;
using System.Linq;
using Common.Reflection;

namespace CodeModels.Models;

public record InterfaceFromReflection(Type ReflectedType)
    : InterfaceModel(ReflectedType.Name,
new NamedValueCollection(ReflectedType.GetProperties(), ReflectedType.GetFields()),
ReflectedType.GetMethods().Select(x => new MethodFromReflection(x)).ToList<IMethod>(),
        new Namespace(ReflectedType.Namespace),
    ReflectionUtil.IsStatic(ReflectedType));
