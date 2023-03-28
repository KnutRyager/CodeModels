namespace Common.Interfaces;

/// <summary>
/// Represent something that can be converted to type <see cref="T"/>.
/// </summary>
/// <typeparam name="T">Target type</typeparam>
public interface IConvertibleTo<T>
    where T : class
{
    /// <summary>
    /// Convert to type <see cref="T"/>.
    /// </summary>
    /// <param name="typeSpecifier">A dummy parameter to ensure
    /// a unique signature when implementing multiple conversions.</param>
    T ToTypeCollection<U>(U? typeSpecifier = default) where U : T;
}
