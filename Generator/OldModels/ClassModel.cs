using System.Collections.Generic;


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
    }
}