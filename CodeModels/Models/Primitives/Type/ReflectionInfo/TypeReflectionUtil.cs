using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeModels.Models.ProgramModels;
using Microsoft.CodeAnalysis;

namespace CodeModels.Models.Reflection;

public static class TypeReflectionUtil
{
    public static IMemberInfo Map(MemberInfo info) => info switch
    {
        FieldInfo field => Map(field),
        PropertyInfo property => Map(property),
        MethodInfo method => Map(method),
        ConstructorInfo constructor => Map(constructor),
        _ => throw new NotImplementedException()
    };

    public static MemberInfo Map(IMemberInfo info) => info switch
    {
        ReflectedFieldInfo field => Map(field),
        ReflectedPropertyInfo property => Map(property),
        ReflectedMethodInfo method => Map(method),
        ReflectedConstructorInfo constructor => Map(constructor),
        _ => throw new NotImplementedException()
    };

    public static Type Map(ITypeInfo info) => info switch
    {
        TypeInfo type => type.Type,
        _ => throw new NotImplementedException()
    };
    public static FieldInfo Map(IFieldInfo info) => info switch
    {
        ReflectedFieldInfo field => field.Info,
        _ => throw new NotImplementedException()
    };
    public static PropertyInfo Map(IPropertyInfo info) => info switch
    {
        ReflectedPropertyInfo property => property.Info,
        _ => throw new NotImplementedException()
    };
    public static MethodInfo Map(IMethodInfo info) => info switch
    {
        ReflectedMethodInfo method => method.Info,
        _ => throw new NotImplementedException()
    };
    public static ConstructorInfo Map(IConstructorInfo info) => info switch
    {
        ReflectedConstructorInfo constructor => constructor.Info,
        _ => throw new NotImplementedException()
    };
    public static ParameterInfo Map(IParameterInfo info) => info switch
    {
        ReflectedParameterInfo parameter => parameter.Info,
        _ => throw new NotImplementedException()
    };
    public static System.Reflection.ICustomAttributeProvider Map(ICustomAttributeProvider info) => info switch
    {
        ReflectedCustomAttributeProvider provider => provider.Info,
        _ => throw new NotImplementedException()
    };

    public static ITypeSymbol MapToSymbol(ITypeInfo info) => info switch
    {
        SymbolTypeInfo symbol => symbol.Type,
        _ => throw new NotImplementedException()
    };
    public static IFieldSymbol MapToSymbol(IFieldInfo info) => info switch
    {
        SymbolFieldInfo symbol => symbol.Info,
        _ => throw new NotImplementedException()
    };
    public static IPropertySymbol MapToSymbol(IPropertyInfo info) => info switch
    {
        SymbolPropertyInfo symbol => symbol.Info,
        _ => throw new NotImplementedException()
    };
    public static IMethodSymbol MapToSymbol(IMethodInfo info) => info switch
    {
        SymbolMethodInfo symbol => symbol.Info,
        _ => throw new NotImplementedException()
    };
    public static IParameterSymbol MapToSymbol(IParameterInfo info) => info switch
    {
        SymbolParameterInfo symbol => symbol.Info,
        _ => throw new NotImplementedException()
    };

    public static TypeInfo Map(Type info) => new(info);
    public static ReflectedFieldInfo Map(FieldInfo info) => new(info);
    public static ReflectedPropertyInfo Map(PropertyInfo info) => new(info);
    public static ReflectedMethodInfo Map(MethodInfo info) => new(info);
    public static ReflectedConstructorInfo Map(ConstructorInfo info) => new(info);
    public static ReflectedParameterInfo Map(ParameterInfo info) => new(info);
    public static ReflectedCustomAttributeProvider Map(System.Reflection.ICustomAttributeProvider info) => new(info);

    public static IMemberInfo Map(ISymbol symbol, IProgramContext context) => symbol switch
    {
        INamedTypeSymbol type => Map(type, context),
        IFieldSymbol field => Map(field, context),
        IPropertySymbol property => Map(property, context),
        IMethodSymbol method => Map(method, context),
        _ => throw new NotImplementedException()
    };
    public static SymbolTypeInfo Map(INamedTypeSymbol symbol, IProgramContext context) => new(symbol, context);
    public static SymbolTypeInfo Map(ITypeSymbol symbol, IProgramContext context) => new(symbol, context);
    public static SymbolFieldInfo Map(IFieldSymbol symbol, IProgramContext context) => new(symbol, context);
    public static SymbolPropertyInfo Map(IPropertySymbol symbol, IProgramContext context) => new(symbol, context);
    public static SymbolMethodInfo Map(IMethodSymbol symbol, IProgramContext context) => new(symbol, context);
    public static SymbolConstructorInfo MapConstructor(IMethodSymbol symbol, IProgramContext context) => new(symbol, context);
    public static SymbolParameterInfo Map(IParameterSymbol symbol, IProgramContext context) => new(symbol, context);
    //public static SymbolCustomAttributeProvider Map(System.Reflection.ICustomAttributeProvider info) => new(info);
    public static CustomAttributeData Map(AttributeData data, IProgramContext context) => throw new NotImplementedException();
    public static CustomAttributeData[] Map(IEnumerable<AttributeData> data, IProgramContext context) => data.Select(x => Map(x, context)).ToArray();
    public static PropertyAttributes MapPropertyAttributes(IEnumerable<AttributeData> data, IProgramContext context) => throw new NotImplementedException();
    public static ParameterAttributes MapParameterAttributes(IEnumerable<AttributeData> data, IProgramContext context) => throw new NotImplementedException();
    public static ICustomAttributeProvider MapICustomAttributeProvider(IEnumerable<AttributeData> data, IProgramContext context) => throw new NotImplementedException();
    public static ITypeInfo MapCustomModifier(CustomModifier modifier, IProgramContext context) => throw new NotImplementedException();
    public static ITypeInfo[] MapCustomModifier(IEnumerable<CustomModifier> modifiers, IProgramContext context) => modifiers.Select(x => MapCustomModifier(x, context)).ToArray();

    public static bool Covers(BindingFlags flags, ISymbol symbol) => Covers(flags, symbol.DeclaredAccessibility)
        && (!symbol.IsStatic || flags.HasFlag(BindingFlags.Static)
        && (symbol.IsStatic || flags.HasFlag(BindingFlags.Instance)));
    public static bool Covers(BindingFlags flags, Accessibility accesability) => accesability switch
    {
        Accessibility.NotApplicable => true,
        Accessibility.Private => !flags.HasFlag(BindingFlags.Public),
        Accessibility.ProtectedAndInternal | Accessibility.ProtectedAndFriend => flags.HasFlag(BindingFlags.NonPublic),
        Accessibility.Protected => !flags.HasFlag(BindingFlags.NonPublic),
        Accessibility.Internal | Accessibility.Friend => flags.HasFlag(BindingFlags.NonPublic),
        Accessibility.ProtectedOrInternal | Accessibility.ProtectedOrFriend => flags.HasFlag(BindingFlags.NonPublic),
        Accessibility.Public => flags.HasFlag(BindingFlags.Public),
        _ => throw new NotImplementedException()
    };

    public static ITypeInfo[] Map(IEnumerable<Type> info) => info.Select(x => Map(x)).ToArray();
    public static IMemberInfo[] Map(IEnumerable<MemberInfo> info) => info.Select(x => Map(x)).ToArray();
    public static IFieldInfo[] Map(IEnumerable<FieldInfo> info) => info.Select(x => Map(x)).ToArray();
    public static IPropertyInfo[] Map(IEnumerable<PropertyInfo> info) => info.Select(x => Map(x)).ToArray();
    public static IMethodInfo[] Map(IEnumerable<MethodInfo> info) => info.Select(x => Map(x)).ToArray();
    public static IConstructorInfo[] Map(IEnumerable<ConstructorInfo> info) => info.Select(x => Map(x)).ToArray();
    public static IParameterInfo[] Map(IEnumerable<ParameterInfo> info) => info.Select(x => Map(x)).ToArray();

    public static Type[] Map(IEnumerable<ITypeInfo> info) => info.Select(x => Map(x)).ToArray();
    public static MemberInfo[] Map(IEnumerable<IMemberInfo> info) => info.Select(x => Map(x)).ToArray();
    public static FieldInfo[] Map(IEnumerable<IFieldInfo> info) => info.Select(x => Map(x)).ToArray();
    public static PropertyInfo[] Map(IEnumerable<IPropertyInfo> info) => info.Select(x => Map(x)).ToArray();
    public static MethodInfo[] Map(IEnumerable<IMethodInfo> info) => info.Select(x => Map(x)).ToArray();
    public static ConstructorInfo[] Map(IEnumerable<IConstructorInfo> info) => info.Select(x => Map(x)).ToArray();
    public static ParameterInfo[] Map(IEnumerable<IParameterInfo> info) => info.Select(x => Map(x)).ToArray();

    public static ITypeSymbol[] MapToSymbol(IEnumerable<ITypeInfo> info) => info.Select(x => MapToSymbol(x)).ToArray();
    public static IFieldSymbol[] MapToSymbol(IEnumerable<IFieldInfo> info) => info.Select(x => MapToSymbol(x)).ToArray();
    public static IPropertySymbol[] MapToSymbol(IEnumerable<IPropertyInfo> info) => info.Select(x => MapToSymbol(x)).ToArray();
    public static IMethodSymbol[] MapToSymbol(IEnumerable<IMethodInfo> info) => info.Select(x => MapToSymbol(x)).ToArray();
    //public static ConstructorInfo[] MapToSymbol(IEnumerable<IConstructorInfo> info) => info.Select(x => Map(x)).ToArray();
    public static IParameterSymbol[] MapToSymbol(IEnumerable<IParameterInfo> info) => info.Select(x => MapToSymbol(x)).ToArray();

    public static IMemberInfo[] Map(IEnumerable<ISymbol> info, IProgramContext context) => info.Select(x => Map(x, context)).ToArray();
    public static ITypeInfo[] Map(IEnumerable<ITypeSymbol> info, IProgramContext context) => info.Select(x => Map(x, context)).ToArray();
    public static IFieldInfo[] Map(IEnumerable<IFieldSymbol> info, IProgramContext context) => info.Select(x => Map(x, context)).ToArray();
    public static IPropertyInfo[] Map(IEnumerable<IPropertySymbol> info, IProgramContext context) => info.Select(x => Map(x, context)).ToArray();
    public static IMethodInfo[] Map(IEnumerable<IMethodSymbol> info, IProgramContext context) => info.Select(x => Map(x, context)).ToArray();
    public static IConstructorInfo[] MapConstructor(IEnumerable<IMethodSymbol> info, IProgramContext context) => info.Select(x => MapConstructor(x, context)).ToArray();
    public static IParameterInfo[] Map(IEnumerable<IParameterSymbol> info, IProgramContext context) => info.Select(x => Map(x, context)).ToArray();
}
