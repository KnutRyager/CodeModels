using System.Collections.Generic;

namespace Common.Reflection;

public static class TypeParsing
{
    public static bool IsMemberAccess(string s) => s.Contains(".");
    public static string NamePart(string s) => IsMemberAccess(s) ? s[(s.LastIndexOf(".") + 1)..] : s;
    public static string PathPart(string s) => IsMemberAccess(s) ? s[..s.LastIndexOf(".")] : "";
    public static bool IsArrayIdentifier(string identifier) => identifier.EndsWith("[]");
    public static bool IsGenericIdentifier(string identifier) => identifier.Contains("<");
    public static bool IsGenericIdentifierOfTypeName(string identifier) => identifier.Contains("`");
    public static string RemoveGenericAndArrayPart(string identifier) => RemoveGenericPart(RemoveArrayPart(identifier));
    public static string RemoveGenericPart(string identifier) => IsGenericIdentifier(identifier) ? identifier[..identifier.IndexOf('<')] : identifier;
    public static string RemoveGenericPartOfTypeName(string identifier) => IsGenericIdentifierOfTypeName(identifier) ? identifier[..identifier.IndexOf('`')] : identifier;
    public static string RemoveArrayPart(string identifier) => IsArrayIdentifier(identifier) ? identifier[..identifier.LastIndexOf('[')] : identifier;
    public static string RemoveEnclosingBrackets(string type) => type.StartsWith("[") && type.EndsWith("]") ? type[1..(type.Length - 1)] : type;
    public static string RemoveAssemblyInfo(string type) => type.IndexOf(",") > 0 && (type.IndexOf("]") < 0 || type.IndexOf(",") > type.IndexOf("]")) ? type[..type.IndexOf(",")] : type;

    public static ParsedGenericType ParseGenericType(string identifier) => identifier.Contains("`")
        ? ParseGenericTypeOfTypeName(identifier)
        : new(RemoveGenericPart(identifier), ParseGenericParameters(identifier));
    public static ParsedGenericType ParseGenericTypeOfTypeName(string identifier) => new(RemoveGenericPartOfTypeName(identifier), ParseGenericParametersOfTypeName(identifier));

    public static List<ParsedGenericType> ParseGenericParameters(string identifier)
    {
        var genericStartIndex = identifier.IndexOf("<");
        var innerTypes = new List<ParsedGenericType>();
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
                innerTypes.Add(new ParsedGenericType(RemoveGenericPart(ReflectionSerialization.NormalizeType(subIdentifier)), ParseGenericParameters(subIdentifier)));
                innerTypeStartIndex = i + 1;
            }
        }
        return innerTypes;
    }

    public static List<ParsedGenericType> ParseGenericParametersOfTypeName(string identifier)
    {
        var genericStartIndex = identifier.IndexOf("[");
        var innerTypes = new List<ParsedGenericType>();
        if (genericStartIndex == -1) return innerTypes;
        var genericEndIndex = identifier.LastIndexOf("]");
        var inner = identifier.Substring(genericStartIndex + 1, genericEndIndex - (genericStartIndex + 1));
        var depth = 0;
        var innerTypeStartIndex = 0;
        for (var i = 0; i < inner.Length + 1; i++)
        {
            var c = i < inner.Length ? inner[i] : '_';
            if (c == '[') depth++;
            else if (c == ']') depth--;
            else if ((c == ',' && depth == 0) || i == inner.Length)
            {
                var subIdentifier = RemoveEnclosingBrackets(inner[innerTypeStartIndex..i]);
                subIdentifier = RemoveAssemblyInfo(subIdentifier);
                innerTypes.Add(new ParsedGenericType(RemoveGenericPartOfTypeName(
                    ReflectionSerialization.NormalizeTypeOfTypeName(subIdentifier)),
                    ParseGenericParametersOfTypeName(subIdentifier)));
                innerTypeStartIndex = i + 1;
            }
        }
        return innerTypes;
    }
}