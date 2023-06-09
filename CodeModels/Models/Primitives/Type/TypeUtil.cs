﻿using System.Collections.Generic;
using System.Linq;
using CodeModels.Factory;
using CodeModels.Models.Primitives.Expression.Abstract;
using Common.DataStructures;
using Common.Reflection;

namespace CodeModels.Models;

public static class TypeUtil
{
    public static IType FindCommonType(IEnumerable<IType> types)
    {
        if (types.Count() == 0) return CodeModelFactory.QuickType("object");
        var isMulti = types.Any(x => x.IsMulti);
        var isOptional = types.Any(x => !x.Required);
        var disinctTypes = types.Select(x => x.TypeName).Distinct();
        var specificType = types.Distinct().Count() is 1 ? types.First().Name : "object";
        //var specificType = disinctTypes.Count() == 1 ? types.First().GetMostSpecificType()
        //    : disinctTypes.Count() == 0 ? "int"
        //    : types.Any(x => x.GetMostSpecificType() is "object") ? "object"
        //    : types.Any(x => x.GetMostSpecificType() is "string") ? "string"
        //    : "int";
        // TODO TYPE FIX
        var type = CodeModelFactory.QuickType(specificType);
        return isOptional ? type.ToOptionalType() : type;
        //return CodeModelFactory.QuickType(specificType, !isOptional);
        //return new QuickType(specificType, !isOptional, isMulti);
    }
    public static IType FindCommonType(IEnumerable<IExpression> expressions) => FindCommonType(expressions.Select(x => x.Get_Type()));
    //=> expressions.Select(x => x.Type).Distinct().Count() is 1
    //? expressions.First().Type : Type(typeof(object));


    public static EqualityList<IType> ParseGenericParameters(string identifier) => identifier.LastOrDefault() is '?'
        ? new EqualityList<IType>() { CodeModelTypeParsing.Parse(identifier[..^1]) }
        : new(TypeParsing.ParseGenericParameters(identifier).Select(TypeFromParsedGenericType));
    public static IType TypeFromParsedGenericType(ParsedGenericType parsed) => CodeModelFactory.QuickType(parsed.Name, parsed.Parameters.Select(TypeFromParsedGenericType));

}
