using System;
using System.Globalization;
using System.Reflection;

namespace CodeModels.Models.Reflection;

public interface IFieldInfo : IMemberInfo
{
    bool IsSpecialName { get; }
     bool IsSecurityTransparent { get; }
     bool IsSecuritySafeCritical { get; }
     bool IsSecurityCritical { get; }
    bool IsPublic { get; }
    bool IsPrivate { get; }
    bool IsPinvokeImpl { get; }
    bool IsNotSerialized { get; }
    bool IsLiteral { get; }
    bool IsInitOnly { get; }
    bool IsFamilyOrAssembly { get; }
    bool IsFamilyAndAssembly { get; }
    bool IsFamily { get; }
    bool IsAssembly { get; }
    abstract ITypeInfo FieldType { get; }
    abstract RuntimeFieldHandle FieldHandle { get; }
    abstract FieldAttributes Attributes { get; }
    bool IsStatic { get; }
     ITypeInfo[] GetOptionalCustomModifiers();
     object GetRawConstantValue();
    ITypeInfo[] GetRequiredCustomModifiers();
    abstract object GetValue(object obj);
     object GetValueDirect(TypedReference obj);
    void SetValue(object obj, object value);
    abstract void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, CultureInfo culture);
     void SetValueDirect(TypedReference obj, object value);
}
