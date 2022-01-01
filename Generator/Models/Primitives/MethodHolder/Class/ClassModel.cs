using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models
{
    public abstract record ClassModel(string Identifier, PropertyCollection Properties, List<IMethod> Methods,
         Namespace? Namespace = null, bool IsStatic = false)
        : MethodHolder(Identifier, Properties, Methods, Namespace,
            IsStatic ? Modifier.Static : Modifier.None, IsStatic ? Modifier.Static : Modifier.None, IsStatic)
    {
        public static ClassModel Parse(ClassDeclarationSyntax @class, NamespaceDeclarationSyntax? @namespace) =>
           @class.IsStatic() ? new StaticClass(@class.Identifier.ValueText,
               null,
               @class.GetMethods().Select(x => new Method(x)),
               @namespace: @namespace == default ? default : new(@namespace))
           : new InstanceClass(@class.Identifier.ValueText,
               null,
               @class.GetMethods().Select(x => new Method(x)),
               @namespace: @namespace == default ? default : new(@namespace));

        public override CSharpSyntaxNode SyntaxNode() => ToClass();
    }
}