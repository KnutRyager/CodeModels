using System;
using System.Globalization;
using System.Reflection;
using static CodeModels.Models.Reflection.TypeReflectionUtil;
using Microsoft.CodeAnalysis;
using CodeModels.Execution;

namespace CodeModels.Models.Reflection;

public abstract record SymbolMethodBase<T>(T Info, IProgramContext Context) : SymbolMemberInfo<T>(Info, Context), IMethodBase where T : IMethodSymbol
{
    public bool IsFamily => throw new NotImplementedException();
    public bool IsFamilyAndAssembly => throw new NotImplementedException();
    public bool IsFamilyOrAssembly => throw new NotImplementedException();
    public bool IsFinal => Info.IsSealed;
    public bool IsGenericMethod => throw new NotImplementedException();
    public bool IsGenericMethodDefinition => throw new NotImplementedException();
    public bool IsHideBySig => throw new NotImplementedException();
    public bool IsPrivate => throw new NotImplementedException();
    public bool IsPublic => throw new NotImplementedException();
    public bool IsSecurityCritical => throw new NotImplementedException();
    public bool IsSecuritySafeCritical => throw new NotImplementedException();
    public bool IsSecurityTransparent => throw new NotImplementedException();
    public bool IsSpecialName => throw new NotImplementedException();
    public bool IsStatic => Info.IsStatic;
    public bool IsConstructor => throw new NotImplementedException();
    public RuntimeMethodHandle MethodHandle => throw new NotImplementedException();
    public bool IsAssembly => throw new NotImplementedException();
    public bool ContainsGenericParameters => GetGenericArguments().Length > 0;
    public bool IsAbstract => Info.IsAbstract;
    public MethodImplAttributes MethodImplementationFlags => throw new NotImplementedException();
    public CallingConventions CallingConvention => throw new NotImplementedException();
    public MethodAttributes Attributes => throw new NotImplementedException();
    public ITypeInfo[] GetGenericArguments() => Map(Info.TypeArguments, Context);
    public MethodBody GetMethodBody() => throw new NotImplementedException();
    public MethodImplAttributes GetMethodImplementationFlags() => throw new NotImplementedException();
    public IParameterInfo[] GetParameters() => throw new NotImplementedException();
    public object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture) => throw new NotImplementedException();
    public object Invoke(object obj, object[] parameters) => throw new NotImplementedException();
}
