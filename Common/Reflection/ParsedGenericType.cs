using System.Collections.Generic;
using System.Linq;

namespace Common.Reflection;

public record ParsedGenericType(string Name, List<ParsedGenericType> Parameters)
{
    public override string ToString()
        => Parameters.Count == 0 ? Name : $"{Name}`{Parameters.Count}[{string.Join(",", Parameters.Select(x => x.ToString()))}]";
    public string ToSimplifiedString(bool removeNamespace = false)
        => Parameters.Count == 0 ? SimplifyName(removeNamespace)
        : TypeParsing.SimplifyNullableType($"{SimplifyName(removeNamespace)}<{string.Join(",", Parameters.Select(x => x.ToSimplifiedString(removeNamespace)))}>");

    private string SimplifyName(bool removeNamespace = false)
    {
        var shorthand = ReflectionSerialization.GetToShortHandName(Name);
        return removeNamespace ? TypeParsing.NamePart(shorthand) : shorthand;
    }
}