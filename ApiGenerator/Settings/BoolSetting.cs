using CodeModels.Models.Primitives.Expression.Reference;

namespace ApiGenerator.Settings;

public enum BoolSetting
{
    Auto,
    True,
    False
}

public static class BoolSettingExtensions
{
    public static bool ReadBool(this BoolSetting setting, bool onAuto = false) => setting switch
    {
        BoolSetting.True => true,
        BoolSetting.False => false,
        BoolSetting.Auto => onAuto,
        _ => throw new NotImplementedException($"Unhandled: '{setting}'")
    };

    public static bool Inherit(this BoolSetting setting, BoolSetting? parent, bool onAuto) => setting switch
    {
        BoolSetting.True or BoolSetting.False => setting.ReadBool(),
        _ => parent?.ReadBool(onAuto) ?? onAuto
    };

    public static bool Inherit<T>(this BoolSetting setting, T? parent, Predicate<T?> parentReader) => setting switch
    {
        BoolSetting.True or BoolSetting.False => setting.ReadBool(),
        _ => parentReader(parent)
    };
}