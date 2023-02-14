using System;
using System.Linq;
using System.Reflection;
using static CodeAnalyzation.Models.Reflection.TypeReflectionUtil;

namespace CodeAnalyzation.Models.Reflection;

public record ReflectedMethodInfo(MethodInfo Info) : ReflectedMethodBase<MethodInfo>(Info), IMethodInfo
{
    public IParameterInfo ReturnParameter => Map(Info.ReturnParameter);
    public ITypeInfo ReturnType => Map(Info.ReturnType);
    public ICustomAttributeProvider ReturnTypeCustomAttributes => Map(Info.ReturnTypeCustomAttributes);
    public Delegate CreateDelegate(ITypeInfo delegateType) => Info.CreateDelegate(Map(delegateType));
    public Delegate CreateDelegate(ITypeInfo delegateType, object target) => Info.CreateDelegate(Map(delegateType), target);
    public IMethodInfo GetBaseDefinition() => Map(Info.GetBaseDefinition());
    public IMethodInfo GetGenericMethodDefinition() => Map(Info.GetGenericMethodDefinition());
    public IMethodInfo MakeGenericMethod(params ITypeInfo[] typeArguments) => Map(Info.MakeGenericMethod(typeArguments.Select(x => Map(x)).ToArray()));
}
