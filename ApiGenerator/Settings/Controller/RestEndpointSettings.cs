namespace ApiGenerator.Settings;

public record RestEndpointSettings(BoolSetting IncludeErrorAnnotation,
    BoolSetting IsStatic)
{
    public static RestEndpointSettings Create(
        BoolSetting includeErrorAnnotation = BoolSetting.Auto,
        BoolSetting isStatic = BoolSetting.Auto) => new(
            IncludeErrorAnnotation: includeErrorAnnotation,
            IsStatic: isStatic);

    public static readonly RestEndpointSettings Default = Create();
}
