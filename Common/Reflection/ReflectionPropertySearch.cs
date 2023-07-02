using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml.Linq;
using Common.Reflection.Member;
using Common.Util;

namespace Common.Reflection;

public static class ReflectionPropertySearch
{
    public static IMemberAccess? FindProperty(Type t, List<MemberInfo>? result)
    {
        foreach (var member in t.GetMembers())
        {
            var type = ReflectionUtil.GetType(member);
            if (type == t) return MemberAccessFactory.Create(member);
        }

        return default;
    }
}
