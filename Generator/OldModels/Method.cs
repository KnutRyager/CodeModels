using System.Collections.Generic;

namespace CodeAnalyzation.Models
{
    public class Method
    {
        public string Identifier { get; set; }
        public IEnumerable<Parameter> Parameters { get; set; }
        public TType ReturnType { get; set; }
        public IEnumerable<Dependency> Dependencies { get; set; }

        public Method(string identifier, IEnumerable<Parameter> parameters, TType returnType, IEnumerable<Dependency> dependencies)
        {
            Identifier = identifier;
            Parameters = parameters;
            ReturnType = returnType;
            Dependencies = dependencies;
        }
    }
}