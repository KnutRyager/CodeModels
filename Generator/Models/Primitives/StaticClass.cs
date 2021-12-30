using System.Collections.Generic;

namespace CodeAnalyzation.Models
{
    public class StaticClass : ClassModel
    {
        public StaticClass(string identifier, PropertyCollection? properties = null, IEnumerable<Method>? methods = null, IEnumerable<AbstractType>? constants = null, Namespace? @namespace = null)
        : base(identifier, properties, methods, constants, @namespace, Modifier.Static, Modifier.Static)
        {
            IsStatic = true;
        }
    }
}