using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models
{
    public class Namespace
    {
        public IEnumerable<string> Parts { get; set; }

        public Namespace(IEnumerable<string> parts)
        {
            Parts = parts;
        }

        public Namespace(NamespaceDeclarationSyntax @namespace) : this(new[] { @namespace.Name.ToString() }) { }

    }
}