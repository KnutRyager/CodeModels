namespace CodeModels.Models.ProgramModels;

public record PropertyModel(
    string Name,
    bool Required,
    IType Type);
