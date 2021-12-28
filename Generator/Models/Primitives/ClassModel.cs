using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models
{
    public class ClassModel : MethodHolder
    {
        public IEnumerable<TType> Constants { get; }

        public ClassModel(string identifier, PropertyCollection? properties = null, IEnumerable<Method>? methods = null, IEnumerable<TType>? constants = null,
            Namespace? @namespace = null, Modifier topLevelModifier = Modifier.None, Modifier memberModifier = Modifier.None)
        : base(identifier, properties: properties, methods: methods, @namespace: @namespace, topLevelModifier: topLevelModifier, memberModifier: memberModifier)
        {
            Constants = constants ?? new List<TType>();
        }

        public static ClassModel Parse(ClassDeclarationSyntax @class, NamespaceDeclarationSyntax? @namespace) =>
           (@class.IsStatic() ? new StaticClass(@class.Identifier.ValueText,
               null,
               @class.GetMethods().Select(x => new Method(x)),
               @namespace: @namespace == default ? default : new Namespace(@namespace))
           : new NonStaticClass(@class.Identifier.ValueText,
               null,
                 @namespace == default ? default : new Namespace(@namespace),
               @class.GetMethods().Select(x => new Method(x))));
    }
}