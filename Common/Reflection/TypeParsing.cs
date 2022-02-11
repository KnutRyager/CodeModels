using System.Collections.Generic;
using System.Linq;

namespace Common.Reflection;

public record ParsedGenericType(string Name, List<ParsedGenericType> Parameters)
{
    public override string ToString() => Parameters.Count == 0 ? Name : $"{Name}`{Parameters.Count}[{string.Join(",", Parameters.Select(x => x.ToString()))}]";
}

public static class TypeParsing
{
    public static bool IsArrayIdentifier(string identifier) => identifier.EndsWith("[]");
    public static bool IsGenericIdentifier(string identifier) => identifier.Contains("<");
    public static string RemoveGenericAndArrayPart(string identifier) => RemoveGenericPart(RemoveArrayPart(identifier));
    public static string RemoveGenericPart(string identifier) => IsGenericIdentifier(identifier) ? identifier[..identifier.IndexOf('<')] : identifier;
    public static string RemoveArrayPart(string identifier) => IsArrayIdentifier(identifier) ? identifier[..identifier.LastIndexOf('[')] : identifier;

    public static ParsedGenericType ParseGenericType(string identifier) => new(RemoveGenericPart(identifier), ParseGenericParameters(identifier));

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
}