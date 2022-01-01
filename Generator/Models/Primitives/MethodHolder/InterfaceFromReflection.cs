using System;
using System.Linq;
using Common.Reflection;

namespace CodeAnalyzation.Models
{
    public record InterfaceFromReflection(Type Type)
        : InterfaceModel(Type.Name,
    new PropertyCollection(Type.GetProperties(), Type.GetFields()),
    Type.GetMethods().Select(x => new MethodFromReflection(x)).ToList<IMethod>(),
            new Namespace(Type.Namespace),
        ReflectionUtil.IsStatic(Type));
}