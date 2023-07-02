using System;
using System.Reflection;

namespace Common.Reflection.Member;

public record MethodMemberAccess(MethodInfo Member, object?[] Arguments)
    : MemberAccess<MethodInfo>(Member)
{
    public override object? Invoke(object? instance)
        => Member.Invoke(instance, Arguments);

    public override Type Type() => Member.ReturnType;
}
