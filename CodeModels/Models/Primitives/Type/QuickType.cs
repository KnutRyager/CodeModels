using System;
using System.Collections.Generic;
using System.Linq;
using Common.DataStructures;
using Common.Reflection;
using Microsoft.CodeAnalysis;

namespace CodeModels.Models;

public abstract record QuickType(string Identifier, EqualityList<IType> GenericTypes, bool Required = true, bool IsMulti = false, Type? ReflectedType = null, ITypeDeclaration? Declaration = null, ITypeSymbol? Symbol = null)
    : AbstractType(Identifier, GenericTypes, Required, IsMulti, ReflectedType, Symbol)
{
    public static QuickType Create(string identifier,
        bool required = true,
        bool isMulti = false,
        Type? type = null,
        ITypeDeclaration? declaration = null,
        ITypeSymbol? symbol = null)
            => Create(identifier, TypeUtil.ParseGenericParameters(identifier), required, isMulti, type, declaration, symbol);

    public static QuickType Create(string identifier,
        IEnumerable<IType> genericTypes,
        bool required = true,
        bool isMulti = false,
        Type? type = null,
        ITypeDeclaration? declaration = null,
        ITypeSymbol? symbol = null)
        => new QuickTypeImp(ReflectionSerialization.GetToShortHandName(TypeParsing.RemoveGenericAndArrayPart(identifier)), genericTypes.ToEqualityList(), required, isMulti || TypeParsing.IsArrayIdentifier(identifier), type, declaration, symbol);

    //public QuickType(string identifier, bool required = true, bool isMulti = false, Type? type = null, ITypeDeclaration? declaration = null, ITypeSymbol? symbol = null)
    //    : this(TypeParsing.RemoveGenericAndArrayPart(identifier), TypeUtil.ParseGenericParameters(identifier), required, isMulti || TypeParsing.IsArrayIdentifier(identifier), type, declaration, symbol) { }
    //public QuickType(string identifier, IEnumerable<IType> genericTypes, bool required = true, bool isMulti = false, Type? type = null, ITypeDeclaration? declaration = null, ITypeSymbol? symbol = null)
    //    : this(TypeParsing.RemoveGenericAndArrayPart(identifier), genericTypes.ToEqualityList(), required, isMulti || TypeParsing.IsArrayIdentifier(identifier), type, declaration, symbol) { }

    public static QuickType ArrayType(IType type) => Create(type.TypeName, type.GenericTypes, type.Required, true, type.ReflectedType?.MakeArrayType());

    public override IType PlainType()
        => this with { IsMulti = false };

    public override string ToString() => $"(Identifier: {TypeName}, GenericTypes: {GenericTypes}, Required: {Required}, IsMulti: {IsMulti}, ReflectedType: {ReflectedType})";
    private record QuickTypeImp(string Identifier,
        EqualityList<IType> GenericTypes,
        bool Required = true,
        bool IsMulti = false,
        Type? ReflectedType = null,
        ITypeDeclaration? Declaration = null,
        ITypeSymbol? Symbol = null)
    : QuickType(
        Identifier: Identifier,
        GenericTypes: GenericTypes,
        Required: Required,
        IsMulti: IsMulti,
        ReflectedType: ReflectedType,
        Declaration: Declaration,
        Symbol: Symbol);
}
