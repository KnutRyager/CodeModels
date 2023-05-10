using System.Collections.Generic;
using System.Linq;

namespace Common.Reflection;

public record ParsedGenericType(string Name, List<ParsedGenericType> Parameters)
{
    public override string ToString()
        => Parameters.Count == 0 ? Name : $"{Name}`{Parameters.Count}[{string.Join(",", Parameters.Select(x => x.ToString()))}]";
    public string ToSimplifiedString(bool removeNamespace = false)
        => Parameters.Count == 0 ? ReflectionSerialization.GetToShortHandName(Name) : $"{ReflectionSerialization.GetToShortHandName(removeNamespace ? TypeParsing.NamePart(Name) : Name)}<{string.Join(",", Parameters.Select(x => x.ToSimplifiedString(removeNamespace)))}>";
}