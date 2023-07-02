namespace Common.Reflection.Member;

public interface IMemberAccess
{
    object? Invoke(object? instance);
}