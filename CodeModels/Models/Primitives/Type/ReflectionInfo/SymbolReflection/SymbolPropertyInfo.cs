using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using CodeModels.Models.ProgramModels;
using Microsoft.CodeAnalysis;
using static CodeModels.Models.Reflection.TypeReflectionUtil;

namespace CodeModels.Models.Reflection;

public record SymbolPropertyInfo(IPropertySymbol Info, IProgramContext Context) : SymbolMemberInfo<IPropertySymbol>(Info, Context), IPropertyInfo
{
    public bool IsSpecialName => throw new NotImplementedException();
    public IMethodInfo GetMethod => Map(Info.GetMethod!, Context);
    public bool CanWrite => !Info.IsReadOnly;
    public bool CanRead => !Info.IsWriteOnly;
    public PropertyAttributes Attributes => MapPropertyAttributes(Info.GetAttributes(), Context);
    public ITypeInfo PropertyType => Map(Info.Type, Context);
    public IMethodInfo SetMethod => Map(Info.SetMethod!, Context);
    public override MemberTypes MemberType => MemberTypes.Property;
    public IMethodInfo[] GetAccessors() => throw new NotImplementedException();
    public IMethodInfo[] GetAccessors(bool non) => throw new NotImplementedException();
    public object GetConstantValue() => throw new NotImplementedException();
    public IMethodInfo GetGetMethod() => Map(Info.GetMethod!, Context);
    public IMethodInfo GetGetMethod(bool non) => Map(Info.GetMethod!, Context);
    public IParameterInfo[] GetIndexParameters() => throw new NotImplementedException();
    public ITypeInfo[] GetOptionalCustomModifiers() => throw new NotImplementedException();
    //=> Info.TypeCustomModifiers.Select(x => Map(x)).ToArray();
    public object GetRawConstantValue() => throw new NotImplementedException();
    public ITypeInfo[] GetRequiredCustomModifiers() => MapCustomModifier(Info.TypeCustomModifiers.Where(x => !x.IsOptional), Context);
    public IMethodInfo GetSetMethod() => Map(Info.SetMethod!, Context);
    public IMethodInfo GetSetMethod(bool non) => Map(Info.SetMethod!, Context);
    public object GetValue(object obj) => throw new NotImplementedException();
    //=> Info.GetValue(obj);
    public object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
         => throw new NotImplementedException();
    //=> Info.GetValue(obj, invokeAttr, binder, index, culture);
    public object GetValue(object obj, object[] index) => throw new NotImplementedException();
    //=> Info.GetValue(obj, index);
    public void SetValue(object obj, object value, object[] index) => throw new NotImplementedException();
    //=> Info.SetValue(obj, value, index);
    public void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
         => throw new NotImplementedException();
    //=> Info.SetValue(obj, value, invokeAttr, binder, index, culture);
    public void SetValue(object obj, object value) => throw new NotImplementedException();
    //=> Info.SetValue(obj, value);
}
