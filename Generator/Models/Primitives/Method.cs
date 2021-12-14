using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models
{
    public class Method
    {
        public string Identifier { get; set; }
        public PropertyCollection Parameters { get; set; }
        public TType ReturnType { get; set; }
        public IEnumerable<Dependency> Dependencies { get; set; }

        public Method(string identifier, PropertyCollection parameters, TType returnType, IEnumerable<Dependency> dependencies)
        {
            Identifier = identifier;
            Parameters = parameters;
            ReturnType = returnType;
            Dependencies = dependencies;
        }

        public Method(MethodDeclarationSyntax method)
            : this(method.GetName(), new PropertyCollection(method), TType.Parse(method.ReturnType), System.Array.Empty<Dependency>()) { }
    }
}