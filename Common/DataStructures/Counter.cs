namespace Common.DataStructures;

/// <summary>
/// A counter that tracks how many times an int has been taken.
/// </summary>
public class Counter
{
    private int _value;

    public Counter(int initialValue = 0) { _value = initialValue; }

    public int Take() => _value++;
}
