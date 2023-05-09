using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Reflection;
using Microsoft.CodeAnalysis;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models;

public static class CodeModelsFromReflection
{
    public static Namespace Namespace(Type type) => new(type.Namespace);

    public static TypeFromReflection Type(Type type) => new(type);

    public static IBaseTypeDeclaration MetodHolder(Type type) => type switch
    {
        { IsInterface: true } => Interface(type),
        { IsEnum: true } => Enum(type),
        _ when ReflectionUtil.IsStatic(type) => StaticClass(type),
        _ => InstanceClass(type)
    };

    public static StaticClassFromReflection StaticClass(Type type) => new(type);
    public static InstanceClassFromReflection InstanceClass(Type type) => new(type);
    public static InterfaceFromReflection Interface(Type type) => new(type);
    public static EnumFromReflection Enum(Type type) => new(type);

    public static List<IMethod> Methods(Type type) => type.GetMethods().Select(x => new MethodFromReflection(x)).ToList<IMethod>();

    public static LiteralExpression Literal(object value) => new(value);

    public static PropertyCollection PropertyCollection(Type type) => new(type);

    public static MethodFromReflection Method(MethodInfo info) => new(info);

}
