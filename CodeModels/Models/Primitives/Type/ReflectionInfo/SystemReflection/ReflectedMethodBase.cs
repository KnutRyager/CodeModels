using System;
using System.Globalization;
using System.Reflection;
using static CodeAnalyzation.Models.Reflection.TypeReflectionUtil;

namespace CodeAnalyzation.Models.Reflection;

public record ReflectedMethodBase<T>(T Info) : ReflectedMemberInfo<T>(Info), IMethodBase where T : MethodBase
{
    public bool IsFamily => Info.IsFamily;
    public bool IsFamilyAndAssembly => Info.IsFamilyAndAssembly;
    public bool IsFamilyOrAssembly => Info.IsFamilyOrAssembly;
    public bool IsFinal => Info.IsFinal;
    public bool IsGenericMethod => Info.IsGenericMethod;
    public bool IsGenericMethodDefinition => Info.IsGenericMethodDefinition;
    public bool IsHideBySig => Info.IsHideBySig;
    public bool IsPrivate => Info.IsPrivate;
    public bool IsPublic => Info.IsPublic;
    public bool IsSecurityCritical => Info.IsSecurityCritical;
    public bool IsSecuritySafeCritical => Info.IsSecuritySafeCritical;
    public bool IsSecurityTransparent => Info.IsSecurityTransparent;
    public bool IsSpecialName => Info.IsSpecialName;
    public bool IsStatic => Info.IsStatic;
    public bool IsConstructor => Info.IsConstructor;
    public RuntimeMethodHandle MethodHandle => Info.MethodHandle;
    public bool IsAssembly => Info.IsAssembly;
    public bool ContainsGenericParameters => Info.ContainsGenericParameters;
    public bool IsAbstract => Info.IsAbstract;
    public MethodImplAttributes MethodImplementationFlags => Info.MethodImplementationFlags;
    public CallingConventions CallingConvention => Info.CallingConvention;
    public MethodAttributes Attributes => Info.Attributes;
    public ITypeInfo[] GetGenericArguments() => Map(Info.GetGenericArguments());
    public MethodBody GetMethodBody() => Info.GetMethodBody();
    public MethodImplAttributes GetMethodImplementationFlags() => Info.GetMethodImplementationFlags();
    public IParameterInfo[] GetParameters() => Map(Info.GetParameters());
    public object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
        => Info.Invoke(obj, invokeAttr, binder, parameters, culture);
    public object Invoke(object obj, object[] parameters) => Info.Invoke(obj, parameters);
}
