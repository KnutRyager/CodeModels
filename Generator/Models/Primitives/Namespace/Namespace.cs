using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public record Namespace(IEnumerable<string> Parts)
{
    public Namespace(params string[] parts) : this(parts.ToList()) { }

    public Namespace(NamespaceDeclarationSyntax @namespace) : this(string.IsNullOrWhiteSpace(@namespace.Name.ToString()) ? new string[] { }
    : new[] { @namespace.Name.ToString() })
    { }
}
