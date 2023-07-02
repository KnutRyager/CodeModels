using System.Collections.Generic;

namespace Common.Reflection.Member;

public record DeepMemberAccess(List<IMemberAccess> Members)
    : IMemberAccess
{
    public object? Invoke(object? instance)
    {
        foreach (var member in Members)
        {
            instance = member.Invoke(instance);
        }
        return instance;
    }
}
