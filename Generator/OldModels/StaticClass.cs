using System.Collections.Generic;

namespace CodeAnalyzation.Models
{
    public class StaticClass : ClassModel
    {

        public StaticClass(string identifier, Namespace? @namespace, IEnumerable<Method>? methods = null, IEnumerable<TType>? constants = null)
        : base(identifier, @namespace, methods, constants)
        {
        }
    }
}