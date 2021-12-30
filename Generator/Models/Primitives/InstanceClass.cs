using System.Collections.Generic;

namespace CodeAnalyzation.Models
{
    public class InstanceClass : ClassModel
    {
        public InstanceClass(string identifier, PropertyCollection? properties = null, Namespace? @namespace = null, IEnumerable<Method>? methods = null,
            IEnumerable<AbstractType>? constants = null)
        : base(identifier, properties, methods, constants, @namespace)
        {
        }
    }
}