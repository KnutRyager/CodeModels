using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml.Linq;
using Common.Algorithms.Search;
using Common.Reflection.Member;
using Common.Util;

namespace Common.Reflection;

public static class ReflectionPropertySearch
{
    public static IMemberAccess? GetMemberAccess(Type start, Type end)
        => FindPath(start, end) is List<IMemberAccess> access ? MemberAccessFactory.Create(access) : null;

    public static List<IMemberAccess>? FindPath(Type start, Type end)
        => start == end ? new List<IMemberAccess>()
        : BreathFirstSearch.FindPathWithEdges(start, end.IsAssignableFrom,
                x => x.GetMembers().Where(x => x is not (ConstructorInfo or Type or EventInfo)
                // && x.Name is not ("GetHashCode" or "Compare" or "CompareOrdinal" or "CompareTo")
                ).ToArray()
            .Select(y => MemberAccessFactory.Create(y)).ToArray(), x => x.Type())
            ?.Skip(1).Select(x => x.Edge).ToList();
}
