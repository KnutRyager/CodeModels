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

        public Namespace(NamespaceDeclarationSyntax? @namespace) : this(string.IsNullOrWhiteSpace(@namespace?.Name?.ToString()) ? new string[] { } : new[] { @namespace.Name.ToString() }) { }

        public override string ToString() => string.Join(".", Parts);
    }
}