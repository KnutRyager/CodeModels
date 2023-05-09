using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using CodeModels.Models.Execution;
using Microsoft.CodeAnalysis;
using static CodeModels.Models.Reflection.TypeReflectionUtil;

namespace CodeModels.Models.Reflection;

public record SymbolTypeInfo(ITypeSymbol Type, IProgramContext Context) : SymbolMemberInfo<ITypeSymbol>(Type, Context), ITypeInfo
{
    public ITypeInfo UnderlyingSystemType => Type is INamedTypeSymbol namedType ? Map(namedType.EnumUnderlyingType ?? namedType.NativeIntegerUnderlyingType ?? namedType.TupleUnderlyingType ?? throw new NotImplementedException(), Context)
        : throw new NotImplementedException();
    public override MemberTypes MemberType => MemberTypes.TypeInfo;
    ITypeInfo IReflectInfo.UnderlyingSystemType => UnderlyingSystemType;

    public IFieldInfo GetField(string name, BindingFlags bindingAttr)
        => (GetSingleMember(name, bindingAttr) as IFieldInfo)!;
    public IFieldInfo[] GetFields(BindingFlags bindingAttr)
        => (GetMembers(bindingAttr).Select(x => (x as IFieldInfo)!)).ToArray();

    public IMemberInfo[] GetMember(string name, BindingFlags bindingAttr)
        => Map(Type.GetMembers().Where(x => Covers(bindingAttr, x) && x.Name == name), Context);
    public IMemberInfo[] GetMembers(BindingFlags bindingAttr)
        => Map(Type.GetMembers().Where(x => Covers(bindingAttr, x)), Context);
    public IMemberInfo GetSingleMember(string name, BindingFlags bindingAttr)
        => GetMember(name, bindingAttr).FirstOrDefault() ?? throw new NotImplementedException();
    public IMemberInfo GetSingleMember(BindingFlags bindingAttr)
        => GetMembers(bindingAttr).FirstOrDefault() ?? throw new NotImplementedException();

    public IMethodInfo GetMethod(string name, BindingFlags bindingAttr)
        => (GetSingleMember(name, bindingAttr) as IMethodInfo)!;
    public IMethodInfo GetMethod(string name, BindingFlags bindingAttr, Binder binder, ITypeInfo[] types, ParameterModifier[] modifiers)
        => (GetSingleMember(name, bindingAttr) as IMethodInfo)!;
    public IMethodInfo[] GetMethods(BindingFlags bindingAttr)
        => (GetMembers(bindingAttr).Select(x => (x as IMethodInfo)!)).ToArray();

    public IPropertyInfo[] GetProperties(BindingFlags bindingAttr)
        => (GetMembers(bindingAttr).Select(x => (x as IPropertyInfo)!)).ToArray();
    public IPropertyInfo GetProperty(string name, BindingFlags bindingAttr)
        => (GetSingleMember(name, bindingAttr) as IPropertyInfo)!;
    public IPropertyInfo GetProperty(string name, BindingFlags bindingAttr, Binder binder, ITypeInfo returnType, ITypeInfo[] types, ParameterModifier[] modifiers)
        => (GetSingleMember(name, bindingAttr) as IPropertyInfo)!;

    public object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
        => GetSingleMember(name, invokeAttr) switch
        {
            IFieldInfo field => field.GetValue(target),
            IPropertyInfo property => property.GetValue(target),
            IMethodInfo method => method.Invoke(target, args),
            _ => throw new NotImplementedException(),
        };
}
