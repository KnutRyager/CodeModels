using System.Collections.Generic;

namespace CodeModels.ProgramModels.ModelModel;

public record ModelModel(
    string Name,
    List<PropertyModel> Properties);
