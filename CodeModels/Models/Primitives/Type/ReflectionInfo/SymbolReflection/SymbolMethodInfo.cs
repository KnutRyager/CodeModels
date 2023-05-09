using System;
using System.Linq;
using CodeModels.Models.Execution;
using Microsoft.CodeAnalysis;
using static CodeModels.Models.Reflection.TypeReflectionUtil;

namespace CodeModels.Models.Reflection;

public record SymbolMethodInfo(IMethodSymbol Info, IProgramContext Context) : SymbolMethodBase<IMethodSymbol>(Info, Context), IMethodInfo
{
    public IParameterInfo ReturnParameter => throw new NotImplementedException();
    public ITypeInfo ReturnType => Map(Info.ReturnType, Context);
    public ICustomAttributeProvider ReturnTypeCustomAttributes => MapICustomAttributeProvider(Info.GetAttributes(), Context);
    public override System.Reflection.MemberTypes MemberType => System.Reflection.MemberTypes.Method;
    public Delegate CreateDelegate(ITypeInfo delegateType) => throw new NotImplementedException();
    public Delegate CreateDelegate(ITypeInfo delegateType, object target) => throw new NotImplementedException();
    public IMethodInfo GetBaseDefinition() => throw new NotImplementedException();
    public IMethodInfo GetGenericMethodDefinition() => throw new NotImplementedException();
    public IMethodInfo MakeGenericMethod(params ITypeInfo[] typeArguments) => throw new NotImplementedException();
}
