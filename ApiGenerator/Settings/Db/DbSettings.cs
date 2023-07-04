namespace ApiGenerator.Settings;

public record DbSettings()
{
    public static DbSettings Create()
        => new();
}
