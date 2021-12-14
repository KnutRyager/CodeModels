using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models
{
    public class ClassModel : Model
    {
        public IEnumerable<Method> Methods { get; }
        public IEnumerable<TType> Constants { get; }

        public ClassModel(string identifier, Namespace? @namespace, IEnumerable<Method>? methods = null, IEnumerable<TType>? constants = null)
        : base(identifier, @namespace)
        {
            Methods = methods ?? new List<Method>();
            Constants = constants ?? new List<TType>();
        }

        public static ClassModel Parse(ClassDeclarationSyntax @class, NamespaceDeclarationSyntax? @namespace) =>
           (@class.IsStatic() ? new StaticClass(@class.Identifier.ValueText,
               new Namespace(@namespace),
               @class.GetMethods().Select(x => new Method(x)))
           : new NonStaticClass(@class.Identifier.ValueText,
               new Namespace(@namespace),
               @class.GetMethods().Select(x => new Method(x))));
    }
}