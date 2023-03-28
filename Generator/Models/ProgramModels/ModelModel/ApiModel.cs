using System;
using System.Collections.Generic;
using CodeAnalyzation.Models.ErDiagram;
namespace CodeAnalyzation.Models.ProgramModels;

public record ModelModel(
    string Name,
    List<PropertyModel> Properties);
