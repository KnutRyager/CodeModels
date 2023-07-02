using System;
using System.Collections.Generic;
using System.Reflection;

namespace Common.Reflection.Member;

public static class MemberAccessFactory
{
    public static IMemberAccess Create(MemberInfo member, object?[]? arguments = null) => member switch
    {
        FieldInfo field => new FieldMemberAccess(field),
        PropertyInfo property => new PropertyMemberAccess(property),
        MethodInfo method => new MethodMemberAccess(method, arguments ?? Array.Empty<object?>()),
        _ => throw new NotImplementedException($"Not implemented: '{member?.GetType()}'")
    };

    public static DeepMemberAccess Create(IEnumerable<IMemberAccess> members)
        => DeepMemberAccess.Create(members);
}
