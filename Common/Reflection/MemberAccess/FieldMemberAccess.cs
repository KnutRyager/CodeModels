using System;
using System.Reflection;

namespace Common.Reflection.Member;

public record FieldMemberAccess(FieldInfo Member)
    : MemberAccess<FieldInfo>(Member)
{
    public override object? Invoke(object? instance)
        => Member.GetValue(instance);

    public override Type Type() => Member.FieldType;
}
