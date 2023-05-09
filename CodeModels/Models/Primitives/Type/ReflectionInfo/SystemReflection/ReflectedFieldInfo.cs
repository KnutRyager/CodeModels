using System;
using System.Globalization;
using System.Reflection;
using static CodeModels.Models.Reflection.TypeReflectionUtil;

namespace CodeModels.Models.Reflection;

public record ReflectedFieldInfo(FieldInfo Info) : ReflectedMemberInfo<FieldInfo>(Info), IFieldInfo
{
    public bool IsSpecialName => Info.IsSpecialName;
    public bool IsSecurityTransparent => Info.IsSecurityTransparent;
    public bool IsSecuritySafeCritical => Info.IsSecuritySafeCritical;
    public bool IsSecurityCritical => Info.IsSecurityCritical;
    public bool IsPublic => Info.IsPublic;
    public bool IsPrivate => Info.IsPrivate;
    public bool IsPinvokeImpl => Info.IsPinvokeImpl;
    public bool IsNotSerialized => Info.IsNotSerialized;
    public bool IsLiteral => Info.IsLiteral;
    public bool IsInitOnly => Info.IsInitOnly;
    public bool IsFamilyOrAssembly => Info.IsFamilyOrAssembly;
    public bool IsFamilyAndAssembly => Info.IsFamilyAndAssembly;
    public bool IsFamily => Info.IsFamily;
    public bool IsAssembly => Info.IsAssembly;
    public ITypeInfo FieldType => Map(Info.FieldType);
    public RuntimeFieldHandle FieldHandle => Info.FieldHandle;
    public FieldAttributes Attributes => Info.Attributes;
    public bool IsStatic => Info.IsStatic;
    public ITypeInfo[] GetOptionalCustomModifiers() => Map(Info.GetOptionalCustomModifiers());
    public object GetRawConstantValue() => Info.GetRawConstantValue();
    public ITypeInfo[] GetRequiredCustomModifiers() => Map(Info.GetRequiredCustomModifiers());
    public object GetValue(object obj) => Info.GetValue(obj);
    public object GetValueDirect(TypedReference obj) => Info.GetValueDirect(obj);
    public void SetValue(object obj, object value) => Info.SetValue(obj, value);
    public void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, CultureInfo culture)
        => Info.SetValue(obj, value, invokeAttr, binder, culture);
    public void SetValueDirect(TypedReference obj, object value) => Info.SetValueDirect(obj,value);
    //static bool operator ==(ReflectedFieldInfo left, ReflectedFieldInfo right) => left.Equals(right);
    //static bool operator !=(ReflectedFieldInfo left, ReflectedFieldInfo right) => !left.Equals(right);
}
