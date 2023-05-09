using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using CodeModels.Models.ProgramModels;
using Microsoft.CodeAnalysis;
using static CodeModels.Models.Reflection.TypeReflectionUtil;

namespace CodeModels.Models.Reflection;

public record SymbolFieldInfo(IFieldSymbol Info, IProgramContext Context) : SymbolMemberInfo<IFieldSymbol>(Info, Context), IFieldInfo
{
    public bool IsSpecialName => throw new NotImplementedException();
    public bool IsSecurityTransparent => throw new NotImplementedException();
    public bool IsSecuritySafeCritical => throw new NotImplementedException();
    public bool IsSecurityCritical => throw new NotImplementedException();
    public bool IsPublic => Info.DeclaredAccessibility.HasFlag(Accessibility.Public);
    public bool IsPrivate => Info.DeclaredAccessibility.HasFlag(Accessibility.Private);
    public bool IsPinvokeImpl => throw new NotImplementedException();
    public bool IsNotSerialized => throw new NotImplementedException();
    public bool IsLiteral => Info.HasConstantValue;
    public bool IsInitOnly => throw new NotImplementedException();
    public bool IsFamilyOrAssembly => throw new NotImplementedException();
    public bool IsFamilyAndAssembly => throw new NotImplementedException();
    public bool IsFamily => throw new NotImplementedException();
    public bool IsAssembly => throw new NotImplementedException();
    public ITypeInfo FieldType => Map(Info.Type, Context);
    public RuntimeFieldHandle FieldHandle => throw new NotImplementedException();
    public FieldAttributes Attributes => throw new NotImplementedException();
    public bool IsStatic => Info.IsStatic;
    public override MemberTypes MemberType => MemberTypes.Field;
    public ITypeInfo[] GetOptionalCustomModifiers() => MapCustomModifier(Info.CustomModifiers.Where(x => x.IsOptional), Context);
    public object GetRawConstantValue() => Info.ConstantValue!;
    public ITypeInfo[] GetRequiredCustomModifiers() => MapCustomModifier(Info.CustomModifiers.Where(x => !x.IsOptional), Context);
    public object GetValue(object obj) => throw new NotImplementedException();
    public object GetValueDirect(TypedReference obj) => throw new NotImplementedException();
    public void SetValue(object obj, object value) => throw new NotImplementedException();
    public void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, CultureInfo culture)
        => throw new NotImplementedException();
    public void SetValueDirect(TypedReference obj, object value) => throw new NotImplementedException();
}
