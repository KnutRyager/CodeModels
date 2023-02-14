using System.Globalization;
using System.Linq;
using System.Reflection;
using static CodeAnalyzation.Models.Reflection.TypeReflectionUtil;

namespace CodeAnalyzation.Models.Reflection;

public record ReflectedPropertyInfo(PropertyInfo Info) : ReflectedMemberInfo<PropertyInfo>(Info), IPropertyInfo
{
    public bool IsSpecialName => Info.IsSpecialName;
    public IMethodInfo GetMethod => Map(Info.GetMethod);
    public bool CanWrite => Info.CanWrite;
    public bool CanRead => Info.CanRead;
    public PropertyAttributes Attributes => Info.Attributes;
    public ITypeInfo PropertyType => Map(Info.PropertyType);
    public IMethodInfo SetMethod => Map(Info.SetMethod);
    public IMethodInfo[] GetAccessors() => Info.GetAccessors().Select(x => Map(x)).ToArray();
    public IMethodInfo[] GetAccessors(bool non) => Info.GetAccessors(non).Select(x => Map(x)).ToArray();
    public object GetConstantValue() => Info.GetConstantValue();
    public IMethodInfo GetGetMethod() => Map(Info.GetGetMethod());
    public IMethodInfo GetGetMethod(bool non) => Map(Info.GetGetMethod(non));
    public IParameterInfo[] GetIndexParameters() => Info.GetIndexParameters().Select(x => Map(x)).ToArray();
    public ITypeInfo[] GetOptionalCustomModifiers() => Info.GetOptionalCustomModifiers().Select(x => Map(x)).ToArray();
    public object GetRawConstantValue() => Info.GetRawConstantValue();
    public ITypeInfo[] GetRequiredCustomModifiers() => Info.GetRequiredCustomModifiers().Select(x => Map(x)).ToArray();
    public IMethodInfo GetSetMethod() => Map(Info.GetSetMethod());
    public IMethodInfo GetSetMethod(bool non) => Map(Info.GetSetMethod(non));
    public object GetValue(object obj) => Info.GetValue(obj);
    public object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
        => Info.GetValue(obj, invokeAttr, binder, index, culture);
    public object GetValue(object obj, object[] index) => Info.GetValue(obj, index);
    public void SetValue(object obj, object value, object[] index) => Info.SetValue(obj, value, index);
    public void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
        => Info.SetValue(obj, value, invokeAttr, binder, index, culture);
    public void SetValue(object obj, object value) => Info.SetValue(obj, value);
}
