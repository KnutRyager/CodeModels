namespace CodeModels.ProgramModels.Options;

public record GeneratorOptions(
    NamespaceOptions NamespaceOptions,
    bool AllowMultipleMembersInFile)
{
    public static readonly GeneratorOptions Default = new(
        NamespaceOptions: NamespaceOptions.UsingStatement,
        AllowMultipleMembersInFile: false);
}
