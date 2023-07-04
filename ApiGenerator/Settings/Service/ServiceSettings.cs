namespace ApiGenerator.Settings;

public record ServiceSettings(
        ServiceNamePrefix NamePrefix,
        BoolSetting IncludeGet,
        BoolSetting IncludeCreate,
        BoolSetting IncludeUpdate,
        BoolSetting IncludeDelete)
{
    public static ServiceSettings Create(
        ServiceNamePrefix namePrefix = ServiceNamePrefix.Service,
        BoolSetting includeGet = BoolSetting.Auto,
        BoolSetting includeCreate = BoolSetting.Auto,
        BoolSetting includeUpdate = BoolSetting.Auto,
        BoolSetting includeDelete = BoolSetting.Auto) => new(
            NamePrefix: namePrefix,
            IncludeGet: includeGet,
            IncludeCreate: includeCreate,
            IncludeUpdate: includeUpdate,
            IncludeDelete: includeDelete);

    public static readonly ServiceSettings Default = Create();
}
