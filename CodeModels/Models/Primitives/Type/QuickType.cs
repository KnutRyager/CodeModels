using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CodeModels.Factory;
using Common.DataStructures;
using Common.Reflection;
using Microsoft.CodeAnalysis;

namespace CodeModels.Models;

public abstract record QuickType(string TypeName, bool Required, bool IsMulti, EqualityList<IType> GenericTypes, Type? ReflectedType = null, ITypeSymbol? Symbol = null)
    : AbstractType(TypeName, GenericTypes, Required, IsMulti, ReflectedType, Symbol)
{
    public static QuickType Create(string identifier,
        Type? type = null,
        ITypeSymbol? symbol = null)
            => Create(identifier, TypeUtil.ParseGenericParameters(identifier), type, symbol);

    public static QuickType Create(string identifier,
        IEnumerable<IType> genericTypes,
        Type? type = null,
        ITypeSymbol? symbol = null)
    {
        var name = ReflectionSerialization.SimplifyGenericName(identifier);
        name = TypeParsing.RemoveGenericAndArrayPart(name);
        //var name = ReflectionSerialization.GetToShortHandName(TypeParsing.RemoveGenericAndArrayPart(identifier));
        name = TypeParsing.RemoveNullablePart(name);
        var shouldLookUpReflectedType = type is null && ReflectionSerialization.IsShortHandName(name, true, true);
        //name = isMulti ? $"{name}[]" : name;
        if (shouldLookUpReflectedType) type = ReflectionSerialization.DeserializeType(identifier);

        var isMulti = identifier.EndsWith("[]");
        var required = !identifier.EndsWith("?");

        return new QuickTypeImp(name, genericTypes.ToEqualityList(), required, isMulti, type, symbol);
    }

    public static IType ArrayType(IType type) => type.ToMultiType();

    public override IType PlainType() => RemoveNullable().RemoveMulti();

    public override IType ToMultiType() => Create(
            identifier: $"{TypeName}[]",
            type: ReflectedType?.MakeArrayType(),
            symbol: Symbol);

    public override IType ToOptionalType() => Create(
            identifier: $"{TypeName}?",
            //genericTypes: new[] { this },
            type: ReflectedType is null ? null : ReflectionUtil.GetNullableType(ReflectedType),
            symbol: Symbol);

    public QuickType RemoveMulti()
        => IsMulti ? Create(
            identifier: TypeName,
            type: ReflectedType is null ? null : ReflectedType.GetElementType(),
            symbol: Symbol) : this;

    public QuickType RemoveNullable()
        => !Required ? Create(
            identifier: TypeName[..TypeName.LastIndexOf("?")],
            type: ReflectedType?.GenericTypeArguments.First(),
            symbol: Symbol) : this;

    public override string ToString() => $"(Identifier: {TypeName}, GenericTypes: {GenericTypes}, Required: {Required}, IsMulti: {IsMulti}, ReflectedType: {ReflectedType})";
    private record QuickTypeImp(string Identifier,
        EqualityList<IType> GenericTypes,
        bool Required = true,
        bool IsMulti = false,
        Type? ReflectedType = null,
        ITypeSymbol? Symbol = null)
    : QuickType(
        TypeName: Identifier,
        GenericTypes: GenericTypes,
        Required: Required,
        IsMulti: IsMulti,
        ReflectedType: ReflectedType,
        Symbol: Symbol);
}