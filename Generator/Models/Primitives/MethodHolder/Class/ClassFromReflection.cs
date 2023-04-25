﻿using System;
using System.Linq;
using Common.Reflection;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public record ClassFromReflection(Type ReflectedType) : TypeDeclaration<ClassDeclarationSyntax>(ReflectedType.Name,
    new PropertyCollection(ReflectedType),
    ReflectedType.GetMethods().Select(x => new MethodFromReflection(x)).ToList<IMethod>(),
    new Namespace(ReflectedType.Namespace),
        ReflectionUtil.IsStatic(ReflectedType) ? Modifier.Static : Modifier.None,
        ReflectionUtil.IsStatic(ReflectedType) ? Modifier.Static : Modifier.None)
{
    public override InstantiatedObject CreateInstance()
    {
        throw new NotImplementedException();
    }

    public override ClassDeclarationSyntax Syntax() => ToClass();
}
