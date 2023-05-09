using CodeModels.Models;

namespace CodeModels.ProgramModels.ModelModel;

public record PropertyModel(
    string Name,
    bool Required,
    IType Type);
