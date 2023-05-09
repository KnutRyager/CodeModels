using System.Collections.Generic;

namespace CodeModels.Models;

public record CodeGenerationContext(Namespace? Namespace, List<Namespace> UsingNamespaces);