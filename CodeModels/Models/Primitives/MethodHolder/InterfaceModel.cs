using System.Collections.Generic;
using CodeModels.AbstractCodeModels.Collection;
using Common.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.AbstractCodeModelFactory;
using static CodeModels.Factory.CodeModelFactory;

namespace CodeModels.Models;

public record InterfaceModel(string Identifier, NamedValueCollection Properties, List<IMethod> Methods,
    Namespace? Namespace = null, bool IsStatic = false)
    : TypeDeclaration<InterfaceDeclarationSyntax>(Identifier, Properties, Methods, Namespace,
        Modifier.Public.SetFlags(IsStatic ? Modifier.Static : Modifier.None),
        IsStatic ? Modifier.Static : Modifier.None)
{
    public InterfaceModel(string identifier, NamedValueCollection? properties = null,
        IEnumerable<IMethod>? methods = null, Namespace? @namespace = null)
    : this(identifier, NamedValues(properties), List(methods), @namespace) { }

    public override IInstantiatedObject CreateInstance()
    {
        throw new System.NotImplementedException();
    }

    public override InterfaceDeclarationSyntax Syntax() => ToInterface();
}
