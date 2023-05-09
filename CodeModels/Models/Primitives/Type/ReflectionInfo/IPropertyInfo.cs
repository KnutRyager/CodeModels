using System.Globalization;
using System.Reflection;

namespace CodeModels.Models.Reflection;

public interface IPropertyInfo : IMemberInfo
{
    bool IsSpecialName { get; }
    IMethodInfo GetMethod { get; }
    abstract bool CanWrite { get; }
    abstract bool CanRead { get; }
    abstract PropertyAttributes Attributes { get; }
    abstract ITypeInfo PropertyType { get; }
    IMethodInfo SetMethod { get; }
    IMethodInfo[] GetAccessors();
    abstract IMethodInfo[] GetAccessors(bool non);
    object GetConstantValue();
    IMethodInfo GetGetMethod();
    abstract IMethodInfo GetGetMethod(bool non);
    abstract IParameterInfo[] GetIndexParameters();
    ITypeInfo[] GetOptionalCustomModifiers();
    object GetRawConstantValue();
    ITypeInfo[] GetRequiredCustomModifiers();
    IMethodInfo GetSetMethod();
    abstract IMethodInfo GetSetMethod(bool non);
    object GetValue(object obj);
    abstract object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture);
    object GetValue(object obj, object[] index);
    void SetValue(object obj, object value, object[] index);
    abstract void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture);
    void SetValue(object obj, object value);
    //static bool operator ==(PropertyInfo left, PropertyInfo right);
    //static bool operator !=(PropertyInfo left, PropertyInfo right);

}
