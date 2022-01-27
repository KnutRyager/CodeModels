using System.Collections.Generic;
using System.Linq;
using Common.DataStructures;

namespace CodeAnalyzation.Models;

public static class TypeUtil
{
    public static IType FindCommonType(IEnumerable<IType> types)
    {
        if (types.Count() == 0) return new QuickType("object");
        var isMulti = types.Any(x => x.IsMulti);
        var isOptional = types.Any(x => !x.Required);
        var disinctTypes = types.Select(x => x.Identifier).Distinct();
        var specificType = types.Distinct().Count() is 1 ? types.First().Name : "object";
        //var specificType = disinctTypes.Count() == 1 ? types.First().GetMostSpecificType()
        //    : disinctTypes.Count() == 0 ? "int"
        //    : types.Any(x => x.GetMostSpecificType() is "object") ? "object"
        //    : types.Any(x => x.GetMostSpecificType() is "string") ? "string"
        //    : "int";
        return new QuickType(specificType, !isOptional);
        //return new QuickType(specificType, !isOptional, isMulti);
    }
    public static IType FindCommonType(IEnumerable<IExpression> expressions) => FindCommonType(expressions.Select(x => x.Type));
    //=> expressions.Select(x => x.Type).Distinct().Count() is 1
    //? expressions.First().Type : Type(typeof(object));

    public static bool IsArrayIdentifier(string identifier) => identifier.EndsWith("[]");
    public static bool IsGenericIdentifier(string identifier) => identifier.Contains("<");
    public static string RemoveGenericAndArrayPart(string identifier) => RemoveGenericPart(RemoveArrayPart(identifier));
    public static string RemoveGenericPart(string identifier) => IsGenericIdentifier(identifier) ? identifier[..identifier.IndexOf('<')] : identifier;
    public static string RemoveArrayPart(string identifier) => IsArrayIdentifier(identifier) ? identifier[..identifier.LastIndexOf('[')] : identifier;

    public static EqualityList<IType> ParseGeneric(string identifier)
    {
        var genericStartIndex = identifier.IndexOf("<");
        var innerTypes = new EqualityList<IType>();
        if (genericStartIndex == -1) return innerTypes;
        var genericEndIndex = identifier.LastIndexOf(">");
        var inner = identifier.Substring(genericStartIndex + 1, genericEndIndex - (genericStartIndex + 1));
        var depth = 0;
        var innerTypeStartIndex = 0;
        for (var i = 0; i < inner.Length + 1; i++)
        {
            var c = i < inner.Length ? inner[i] : '_';
            if (c == '<') depth++;
            else if (c == '>') depth--;
            else if ((c == ',' && depth == 0) || i == inner.Length)
            {
                var subIdentifier = inner[innerTypeStartIndex..i];
                innerTypes.Add(new QuickType(RemoveGenericPart(subIdentifier), ParseGeneric(subIdentifier)));
                innerTypeStartIndex = i + 1;
            }
        }
        return innerTypes;
    }
}
