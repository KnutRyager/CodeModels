using CodeModels.Reflection;
using Common.DataStructures;
using Common.Reflection;
using Microsoft.CodeAnalysis;

namespace CodeModels.Models;

public record TypeFromSymbol(ITypeSymbol Symbol, System.Type? ReflectedType)   // TODO: Generics, required, isMulti
    : AbstractType(ReflectionSerialization.GetToShortHandName(Symbol.Name), new EqualityList<IType>(), true, false,
        ReflectedType ?? (SymbolUtils.IsNewDefined(Symbol) ? null : SemanticReflection.GetType(Symbol)))
{
    public static TypeFromSymbol Create(ITypeSymbol symbol, System.Type? reflectedType = default)
        => new(symbol, reflectedType);

    public override System.Type? GetReflectedType() => SemanticReflection.GetType(Symbol!);

    public override string GetMostSpecificType() => Symbol!.ToString();

    public virtual bool Equals(TypeFromSymbol other) => other is not null && (ReferenceEquals(this, other) ||
            (TypeName.Equals(other.TypeName)
            && (Symbol?.Equals(other.Symbol, SymbolEqualityComparer.Default) ?? other.Symbol is null)
            && (ReflectedType?.Equals(other.ReflectedType) ?? other.ReflectedType is null)
            && Required.Equals(other.Required)));

    public override int GetHashCode()
    {
        unchecked // Overflow is fine, just wrap
        {
            int hash = 17;
            hash = hash * 23 + TypeName.GetHashCode();
#pragma warning disable RS1024 // Symbols should be compared for equality
            hash = hash * 23 + Symbol?.GetHashCode() ?? 0;
#pragma warning restore RS1024 // Symbols should be compared for equality
            hash = hash * 23 + ReflectedType?.GetHashCode() ?? 0;
            hash = hash * 23 + Required.GetHashCode();
            return hash;
        }
    }

    public override IType PlainType()
    {
        throw new System.NotImplementedException();
    }

    public override IType ToMultiType()
    {
        throw new System.NotImplementedException();
    }

    public override IType ToOptionalType()
    {
        throw new System.NotImplementedException();
    }
}
