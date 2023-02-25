using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Common.Extensions;

public static class EnumExtensions
{
    public static string GetDescription<T>(this T enumerationValue)
      where T : struct
    {
        var type = enumerationValue.GetType();
        if (!type.IsEnum) throw new ArgumentException($"{nameof(enumerationValue)} must be of Enum type", nameof(enumerationValue));
        var memberInfo = type.GetMember(enumerationValue.ToString()!);
        if (memberInfo.Length > 0)
        {
            var attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attrs.Length > 0)
            {
                return ((DescriptionAttribute)attrs[0]).Description;
            }
        }
        return enumerationValue.ToString()!;
    }

    public static string GetDescriptionOld<T>(this T value) where T : struct, Enum
    {
        var name = Enum.GetName(typeof(T), value);
        if (name is not null)
        {
            var field = typeof(T).GetField(name);
            if (field is not null)
            {
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attr)
                {
                    return attr.Description;
                }
            }
        }
        return null!;
    }

    public static T[] GetEnumValues<T>() where T : struct
    {
        var type = typeof(T);
        if (!type.IsEnum) throw new ArgumentException($"{type.Name} must be of Enum type");
        return (T[])type.GetEnumValues();
    }


    public static bool IsFlagSet<T>(this T value, T flag) where T : struct, Enum
    {
        var lValue = Convert.ToInt64(value);
        var lFlag = Convert.ToInt64(flag);
        return (lValue & lFlag) != 0;
    }

    public static IEnumerable<T> GetFlags<T>(this T value) where T : struct, Enum
    {
        foreach (var flag in Enum.GetValues(typeof(T)).Cast<T>())
        {
            if (value.IsFlagSet(flag))
                yield return flag;
        }
    }

    public static T SetFlags<T>(this T value, T flags, bool on) where T : struct, Enum
    {
        var lValue = Convert.ToInt64(value);
        var lFlag = Convert.ToInt64(flags);
        if (on)
        {
            lValue |= lFlag;
        }
        else
        {
            lValue &= (~lFlag);
        }
        return (T)Enum.ToObject(typeof(T), lValue);
    }

    public static T SetFlags<T>(this T value, T flags) where T : struct, Enum => value.SetFlags(flags, true);
    public static T ClearFlags<T>(this T value, T flags) where T : struct, Enum => value.SetFlags(flags, false);
    public static T CombineFlags<T>(this IEnumerable<T> flags) where T : struct
    {
        var lValue = 0L;
        foreach (var flag in flags)
        {
            var lFlag = Convert.ToInt64(flag);
            lValue |= lFlag;
        }
        return (T)Enum.ToObject(typeof(T), lValue);
    }
}
