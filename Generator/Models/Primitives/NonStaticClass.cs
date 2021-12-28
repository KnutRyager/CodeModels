using System.Collections.Generic;

namespace CodeAnalyzation.Models
{
    public class NonStaticClass : ClassModel
    {
        public IEnumerable<TType> Fields { get; }

        public NonStaticClass(string identifier, PropertyCollection? properties = null, Namespace? @namespace = null, IEnumerable<Method>? methods = null,
            IEnumerable<TType>? constants = null, IEnumerable<TType>? fields = null)
        : base(identifier, properties, methods, constants, @namespace)
        {
            Fields = fields ?? new List<TType>();
        }
    }
}