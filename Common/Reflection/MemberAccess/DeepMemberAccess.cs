using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Reflection.Member;

public record DeepMemberAccess(List<IMemberAccess> Members)
    : IMemberAccess
{
    public static DeepMemberAccess Create(IEnumerable<IMemberAccess> members)
        => new(members?.ToList() ?? new List<IMemberAccess>());

    public object? Invoke(object? instance)
    {
        foreach (var member in Members)
        {
            instance = member.Invoke(instance);
        }
        return instance;
    }

    public Type Type() => Members.LastOrDefault()?.Type()!;
}
