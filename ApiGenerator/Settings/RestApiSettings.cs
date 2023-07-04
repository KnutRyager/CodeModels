using Common.Reflection;

namespace ApiGenerator.Settings;

public record RestApiSettings(DbSettings DbSettings,
    ServiceSettings ServiceSettings,
    ControllerSettings ControllerSettings)
{
    public static RestApiSettings Create(DbSettings? dbSettings = default,
    ServiceSettings? serviceSettings = default,
    ControllerSettings? controllerSettings = default)
        => new(dbSettings ?? DbSettings.Create(),
            serviceSettings ?? ServiceSettings.Create(),
            controllerSettings ?? ControllerSettings.Create());

    public static readonly RestApiSettings Default = Create();

    public T GetSettings<T>() where T : class
    {
        var accessor = ReflectionPropertySearch.GetMemberAccessSameNamespace(typeof(RestApiSettings), typeof(T));
        return accessor.Invoke(this) as T;
    }
}

