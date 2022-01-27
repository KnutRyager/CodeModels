using System;
using System.Collections.Generic;
using Common.DataStructures;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace CodeAnalyzation.Models;

public record QuickType(string Identifier, EqualityList<IType> GenericTypes, bool Required = true, bool IsMulti = false, Type? Type = null)
    : AbstractType(Identifier, GenericTypes, Required, IsMulti, Type)
{
    public QuickType(string identifier, bool required = true, bool isMulti = false, Type? type = null)
        : this(TypeUtil.RemoveGenericAndArrayPart(identifier), TypeUtil.ParseGeneric(identifier), required, isMulti || TypeUtil.IsArrayIdentifier(identifier), type) { }
    public QuickType(string identifier, IEnumerable<IType> genericTypes, bool required = true, bool isMulti = false, Type? type = null)
        : this(TypeUtil.RemoveGenericAndArrayPart(identifier), genericTypes.ToEqualityList(), required, isMulti || TypeUtil.IsArrayIdentifier(identifier), type) { }
}
