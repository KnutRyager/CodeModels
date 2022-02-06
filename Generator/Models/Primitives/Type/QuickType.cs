using System;
using System.Collections.Generic;
using Common.DataStructures;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using Common.Reflection;

namespace CodeAnalyzation.Models;

public record QuickType(string Identifier, EqualityList<IType> GenericTypes, bool Required = true, bool IsMulti = false, Type? ReflectedType = null)
    : AbstractType(Identifier, GenericTypes, Required, IsMulti, ReflectedType)
{
    public QuickType(string identifier, bool required = true, bool isMulti = false, Type? type = null)
        : this(TypeParsing.RemoveGenericAndArrayPart(identifier), TypeUtil.ParseGenericParameters(identifier), required, isMulti || TypeParsing.IsArrayIdentifier(identifier), type) { }
    public QuickType(string identifier, IEnumerable<IType> genericTypes, bool required = true, bool isMulti = false, Type? type = null)
        : this(TypeParsing.RemoveGenericAndArrayPart(identifier), genericTypes.ToEqualityList(), required, isMulti || TypeParsing.IsArrayIdentifier(identifier), type) { }

    public static QuickType ArrayType(IType type) => new(type.Identifier, type.GenericTypes, type.Required, true, type.ReflectedType?.MakeArrayType());

    public override string ToString() => $"(Identifier: {Identifier}, GenericTypes: {GenericTypes}, Required: {Required}, IsMulti: {IsMulti}, ReflectedType: {ReflectedType})";
}
