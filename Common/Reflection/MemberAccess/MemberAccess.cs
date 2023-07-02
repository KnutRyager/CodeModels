using System.Reflection;

namespace Common.Reflection.Member;

public abstract record MemberAccess<T>(T Member)
    : IMemberAccess where T : MemberInfo
{
    public abstract object? Invoke(object? instance);
}
