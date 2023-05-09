using System;
using System.Collections.Generic;
using CodeModels.Models.ErDiagram;
namespace CodeModels.Models.ProgramModels;

public record ModelModel(
    string Name,
    List<PropertyModel> Properties);
