using System.Collections.Generic;
using Common.Util;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Models.CodeModelFactory;

namespace CodeAnalyzation.Models;

public record InterfaceModel(string Identifier, PropertyCollection Properties, List<IMethod> Methods,
    Namespace? Namespace = null, bool IsStatic = false)
    : MethodHolder<InterfaceDeclarationSyntax>(Identifier, Properties, Methods, Namespace,
        Modifier.Public.SetFlags(IsStatic ? Modifier.Static : Modifier.None),
        IsStatic ? Modifier.Static : Modifier.None)
{
    public InterfaceModel(string identifier, PropertyCollection? properties = null,
        IEnumerable<IMethod>? methods = null, Namespace? @namespace = null)
    : this(identifier, PropertyCollection(properties), List(methods), @namespace) { }

    public override InterfaceDeclarationSyntax Syntax() => ToInterface();
}
