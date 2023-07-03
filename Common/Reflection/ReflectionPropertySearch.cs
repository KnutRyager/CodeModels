using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Algorithms.Search;
using Common.Reflection.Member;

namespace Common.Reflection;

public static class ReflectionPropertySearch
{
    public static IMemberAccess? GetMemberAccess(Type start, Type end, Predicate<Type>? typeRestriction = null)
        => FindPath(start, end, typeRestriction) is List<IMemberAccess> access ? MemberAccessFactory.Create(access) : null;

    public static IMemberAccess? GetMemberAccessSameNamespace(Type start, Type end)
        => GetMemberAccess(start, end, TypeRestrictions.SameNamespace(start, end));

    public static IMemberAccess? GetMemberAccessNonSystemNamespace(Type start, Type end)
        => GetMemberAccess(start, end, TypeRestrictions.NonSystemNamespace);

    public static List<IMemberAccess>? FindPath(Type start, Type end, Predicate<Type>? typeRestriction = null)
        => start == end ? new List<IMemberAccess>()
        : BreathFirstSearch.FindPathWithEdges(start, end.IsAssignableFrom,
                x => x.GetMembers().Where(x => x is not (ConstructorInfo or Type or EventInfo)
                && (typeRestriction is null || typeRestriction.Invoke(ReflectionUtil.GetType(x)))
                // && x.Name is not ("GetHashCode" or "Compare" or "CompareOrdinal" or "CompareTo")
                ).ToArray()
            .Select(y => MemberAccessFactory.Create(y)).ToArray(), x => x.Type())
            ?.Skip(1).Select(x => x.Edge).ToList();

    public static List<IMemberAccess>? FindPathSameNamespace(Type start, Type end)
        => FindPath(start, end, TypeRestrictions.SameNamespace(start, end));

    public static List<IMemberAccess>? FindPathNonSystemNamespace(Type start, Type end)
        => FindPath(start, end, TypeRestrictions.NonSystemNamespace);

    public class TypeRestrictions
    {
        public static Predicate<Type> SameNamespace(Type start, Type end)
            => x => x.Namespace.StartsWith(start.Namespace) || x.Namespace.StartsWith(end.Namespace);

        public static readonly Predicate<Type> NonSystemNamespace = x => !x.Namespace.StartsWith("System.");
    }
}
