using System;

namespace CodeAnalyzation.Models.Reflection;

public interface IMethodInfo : IMethodBase
{
    IParameterInfo ReturnParameter { get; }
    ITypeInfo ReturnType { get; }
    abstract ICustomAttributeProvider ReturnTypeCustomAttributes { get; }
    Delegate CreateDelegate(ITypeInfo delegateType);
    Delegate CreateDelegate(ITypeInfo delegateType, object target);
    abstract IMethodInfo GetBaseDefinition();
    IMethodInfo GetGenericMethodDefinition();
    IMethodInfo MakeGenericMethod(params ITypeInfo[] typeArguments);
}
