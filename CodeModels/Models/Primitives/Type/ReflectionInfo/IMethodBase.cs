using System;
using System.Globalization;
using System.Reflection;

namespace CodeAnalyzation.Models.Reflection;

public interface IMethodBase : IMemberInfo
{
    bool IsFamily { get; }
    bool IsFamilyAndAssembly { get; }
    bool IsFamilyOrAssembly { get; }
    bool IsFinal { get; }
    bool IsGenericMethod { get; }
    bool IsGenericMethodDefinition { get; }
    bool IsHideBySig { get; }
    bool IsPrivate { get; }
    bool IsPublic { get; }
    bool IsSecurityCritical { get; }
    bool IsSecuritySafeCritical { get; }
    bool IsSecurityTransparent { get; }
    bool IsSpecialName { get; }
    bool IsStatic { get; }
    bool IsConstructor { get; }
    abstract RuntimeMethodHandle MethodHandle { get; }
    bool IsAssembly { get; }
    bool ContainsGenericParameters { get; }
    bool IsAbstract { get; }
    MethodImplAttributes MethodImplementationFlags { get; }
    CallingConventions CallingConvention { get; }
    abstract MethodAttributes Attributes { get; }
    ITypeInfo[] GetGenericArguments();
    MethodBody GetMethodBody();
    abstract MethodImplAttributes GetMethodImplementationFlags();
    abstract IParameterInfo[] GetParameters();
    abstract object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture);
    object Invoke(object obj, object[] parameters);
}
