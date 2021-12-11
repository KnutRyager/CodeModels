using System.Collections.Generic;

namespace CodeAnalyzation.Models
{
    public class NonStaticClass : ClassModel
    {
        public IEnumerable<TType> Fields { get; }

        public NonStaticClass(string identifier, Namespace? @namespace, IEnumerable<Method>? methods = null,
            IEnumerable<TType>? constants = null, IEnumerable<TType>? fields = null)
        : base(identifier, @namespace, methods, constants)
        {
            Fields = fields ?? new List<TType>();
        }
    }
}