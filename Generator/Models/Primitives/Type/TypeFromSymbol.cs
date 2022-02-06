using CodeAnalyzation.Reflection;
using Common.DataStructures;
using Common.Reflection;
using Microsoft.CodeAnalysis;

namespace CodeAnalyzation.Models;

public record TypeFromSymbol(ITypeSymbol Symbol, bool Required = true, bool IsMulti = false)   // TODO: Generics
    : AbstractType(ReflectionSerialization.GetToShortHandName(Symbol.Name), new EqualityList<IType>(), Required, IsMulti)
{
    public override System.Type? GetReflectedType() => SemanticReflection.GetType(Symbol);

    public override string GetMostSpecificType() => Symbol.ToString();

    public virtual bool Equals(TypeFromSymbol other) => other is not null && (ReferenceEquals(this, other) ||
            (Identifier.Equals(other.Identifier)
            && (Symbol?.Equals(other.Symbol, SymbolEqualityComparer.Default) ?? other.Symbol is null)
            && (ReflectedType?.Equals(other.ReflectedType) ?? other.ReflectedType is null)
            && Required.Equals(other.Required)));

    public override int GetHashCode()
    {
        unchecked // Overflow is fine, just wrap
        {
            int hash = 17;
            hash = hash * 23 + Identifier.GetHashCode();
#pragma warning disable RS1024 // Symbols should be compared for equality
            hash = hash * 23 + Symbol?.GetHashCode() ?? 0;
#pragma warning restore RS1024 // Symbols should be compared for equality
            hash = hash * 23 + ReflectedType?.GetHashCode() ?? 0;
            hash = hash * 23 + Required.GetHashCode();
            return hash;
        }
    }
}
