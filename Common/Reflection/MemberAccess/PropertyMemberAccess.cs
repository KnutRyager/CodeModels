using System;
using System.Reflection;

namespace Common.Reflection.Member;

public record PropertyMemberAccess(PropertyInfo Member)
    : MemberAccess<PropertyInfo>(Member)
{
    public override object? Invoke(object? instance)
        => Member.GetValue(instance);

    public override Type Type() => Member.PropertyType;
}
