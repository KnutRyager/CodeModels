using System.Globalization;
using System.Reflection;

namespace CodeModels.Models.Reflection;

public interface IReflectInfo
{
    ITypeInfo UnderlyingSystemType { get; }
    IFieldInfo GetField(string name, BindingFlags bindingAttr);
    IFieldInfo[] GetFields(BindingFlags bindingAttr);
    IMemberInfo[] GetMember(string name, BindingFlags bindingAttr);
    IMemberInfo[] GetMembers(BindingFlags bindingAttr);
    IMethodInfo GetMethod(string name, BindingFlags bindingAttr);
    IMethodInfo GetMethod(string name, BindingFlags bindingAttr, Binder binder, ITypeInfo[] types, ParameterModifier[] modifiers);
    IMethodInfo[] GetMethods(BindingFlags bindingAttr);
    IPropertyInfo[] GetProperties(BindingFlags bindingAttr);
    IPropertyInfo GetProperty(string name, BindingFlags bindingAttr);
    IPropertyInfo GetProperty(string name, BindingFlags bindingAttr, Binder binder, ITypeInfo returnType, ITypeInfo[] types, ParameterModifier[] modifiers);
    object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters);

}
