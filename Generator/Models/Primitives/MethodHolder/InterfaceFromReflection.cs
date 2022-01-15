using System;
using System.Linq;
using Common.Reflection;

namespace CodeAnalyzation.Models;

public record InterfaceFromReflection(Type ReflectedType)
    : InterfaceModel(ReflectedType.Name,
new PropertyCollection(ReflectedType.GetProperties(), ReflectedType.GetFields()),
ReflectedType.GetMethods().Select(x => new MethodFromReflection(x)).ToList<IMethod>(),
        new Namespace(ReflectedType.Namespace),
    ReflectionUtil.IsStatic(ReflectedType));
