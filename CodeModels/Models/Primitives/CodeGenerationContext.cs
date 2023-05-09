using System.Collections.Generic;
using CodeModels.Models;

public record CodeGenerationContext(Namespace? Namespace, List<Namespace> UsingNamespaces);