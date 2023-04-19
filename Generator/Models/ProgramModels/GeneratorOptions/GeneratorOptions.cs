namespace Generator.Models.ProgramModels.GeneratorOptions;

public record GeneratorOptions(
    NamespaceOptions NamespaceOptions,
    bool AllowMultipleMembersInFile)
{
    public static readonly GeneratorOptions Default = new(
        NamespaceOptions: NamespaceOptions.UsingStatement,
        AllowMultipleMembersInFile: false);
}
