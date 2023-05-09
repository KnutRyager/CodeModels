using System;
using System.Collections.Generic;
using CodeModels.Models;
using CodeModels.Models.ErDiagram;

namespace CodeModels.ProgramModels.ModelModel;

public record ModelModel(
    string Name,
    List<PropertyModel> Properties);
