namespace Common.Interfaces;

/// <summary>
/// Util for specifying the type for functions accepting a
/// type specifier to distinguish method signature.
/// </summary>
public class TypeSpecifier
{
    private TypeSpecifier() { }

    /// <summary>
    /// Get a dummy value of T.
    /// </summary>
    /// <typeparam name="T">default</typeparam>
    public static T? Of<T>() => default;

}
