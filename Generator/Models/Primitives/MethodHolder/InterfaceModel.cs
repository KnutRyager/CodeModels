using System.Collections.Generic;
using System.Linq;
using Common.Util;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models
{
    public record InterfaceModel(string Identifier, PropertyCollection Properties, List<Method> Methods,
        List<IType> Constants, Namespace? Namespace = null, bool IsStatic = false)
        : MethodHolder(Identifier, Properties, Methods, Namespace,
            Modifier.Public.SetFlags(IsStatic ? Modifier.Static : Modifier.None), IsStatic ? Modifier.Static : Modifier.None, IsStatic), ICodeModel
    {
        public InterfaceModel(string identifier, PropertyCollection? properties = null, IEnumerable<Method>? methods = null,
            IEnumerable<IType>? constants = null, Namespace? @namespace = null)
        : this(identifier, properties ?? new PropertyCollection(), methods?.ToList() ?? new List<Method>(), constants?.ToList() ?? new List<IType>(), @namespace) { }

        public CSharpSyntaxNode SyntaxNode() => ToInterface();
    }
}