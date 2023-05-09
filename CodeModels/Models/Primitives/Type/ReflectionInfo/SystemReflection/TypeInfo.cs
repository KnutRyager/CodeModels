using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using static CodeModels.Models.Reflection.TypeReflectionUtil;

namespace CodeModels.Models.Reflection;

public record TypeInfo(Type Type) : ReflectedMemberInfo<Type>(Type), ITypeInfo
{
    public ITypeInfo UnderlyingSystemType => Map(Type.UnderlyingSystemType);
    ITypeInfo IReflectInfo.UnderlyingSystemType => Map(Type.UnderlyingSystemType);
    public IFieldInfo GetField(string name, BindingFlags bindingAttr) => Map(Type.GetField(name, bindingAttr));
    public IFieldInfo[] GetFields(BindingFlags bindingAttr) => Type.GetFields(bindingAttr).Select(x => Map(x)).ToArray();
    public IMemberInfo[] GetMember(string name, BindingFlags bindingAttr) => Map(Type.GetMember(name, bindingAttr));
    public IMemberInfo[] GetMembers(BindingFlags bindingAttr) => Map(Type.GetMembers(bindingAttr));
    public IMethodInfo GetMethod(string name, BindingFlags bindingAttr) => Map(Type.GetMethod(name, bindingAttr));
    public IMethodInfo GetMethod(string name, BindingFlags bindingAttr, Binder binder, ITypeInfo[] types, ParameterModifier[] modifiers)
        => Map(Type.GetMethod(name, bindingAttr, binder, types.Select(x => Map(x)).ToArray(), modifiers));
    public IMethodInfo[] GetMethods(BindingFlags bindingAttr) => Map(Type.GetMethods(bindingAttr));
    public IPropertyInfo[] GetProperties(BindingFlags bindingAttr) => Map(Type.GetProperties(bindingAttr));
    public IPropertyInfo GetProperty(string name, BindingFlags bindingAttr) => Map(Type.GetProperty(name, bindingAttr));
    public IPropertyInfo GetProperty(string name, BindingFlags bindingAttr, Binder binder, ITypeInfo returnType, ITypeInfo[] types, ParameterModifier[] modifiers)
        => Map(Type.GetProperty(name, bindingAttr, binder, Map(returnType), Map(types), modifiers));
    public object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
        => Type.InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);
}
