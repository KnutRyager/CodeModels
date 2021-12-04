using Newtonsoft.Json;
using System.Linq.Expressions;

namespace Common.Util;

public static class CopyUtil
{
    private static readonly JsonSerializerSettings jsonSettings = new()
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    };

    public static T FromJson<T>(string json) => JsonConvert.DeserializeObject<T>(json)!;
    public static string ToJson(object? obj) => JsonConvert.SerializeObject(obj, jsonSettings);
    public static T Copy<T>(T obj) => obj.Copy();
    /// <summary>
    /// Make a copy of an object, and set a property to a new value.
    /// </summary>
    public static T With<T, TProp>(T obj, Expression<Func<T, TProp>> getter, TProp value)
    {
        var copy = Copy(obj);
        ExpressionUtil.MakeSetter(getter)(copy, value);
        return copy;
    }
    /// <summary>
    /// Make a copy of objects, and set a property to a new value on each.
    /// </summary>
    public static List<T> With<T, TProp>(IEnumerable<T> obj, Expression<Func<T, TProp>> getter, TProp value) => obj.Select(x => With(x, getter, value)).ToList();
    /// <summary>
    /// Make a copy of an object, and set a property to the default value.
    /// </summary>
    public static T Clear<T, TProp>(T obj, Expression<Func<T, TProp?>> getter) where TProp : struct => With(obj, getter, default);
    /// <summary>
    /// Make a copy of an object, and set a property to the default value.
    /// </summary>
    public static T Clear<T, TProp>(T obj, Expression<Func<T, TProp?>> getter) where TProp : class => With(obj, getter, default);
    /// <summary>
    /// Make a copy of an object, and set a property to 0.
    /// </summary>
    public static T Clear<T>(T obj, Expression<Func<T, int>> getter) => With(obj, getter, 0);
    /// <summary>
    /// Make a copy of an object, and set a property to 0.
    /// </summary>
    public static T Clear<T>(T obj, Expression<Func<T, decimal>> getter) => With(obj, getter, 0);
    /// <summary>
    /// Make a copy of an object, and set a property to 0.
    /// </summary>
    public static T Clear<T>(T obj, Expression<Func<T, double>> getter) => With(obj, getter, 0);
    /// <summary>
    /// Make a copy of an object, and set a property to 0.
    /// </summary>
    public static T Clear<T>(T obj, Expression<Func<T, long>> getter) => With(obj, getter, 0);
    /// <summary>
    /// Make a copy of an object, and set a property to the empty string.
    /// </summary>
    public static T Empty<T>(T obj, Expression<Func<T, string>> getter) => With(obj, getter, "");
    public static Func<object> CastToFuncObject<T>(Func<T> p) => () => p()!;
    public static Action IgnoreResult<T>(Func<T> f) => () => f();
}
