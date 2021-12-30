using System.Collections.Generic;
using CodeAnalyzation.Models;

public record CodeGenerationContext(Namespace? Namespace, List<Namespace> UsingNamespaces);