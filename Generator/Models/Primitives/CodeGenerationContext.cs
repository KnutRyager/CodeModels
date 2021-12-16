using System.Collections.Generic;
using CodeAnalyzation.Models;

public class CodeGenerationContext
{
    public Namespace? Namespace { get; set; }
    public List<Namespace> UsingNamespaces { get; set; }
}