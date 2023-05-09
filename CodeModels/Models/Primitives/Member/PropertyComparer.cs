using System.Collections.Generic;

namespace CodeModels.Models;

public record PropertyComparer(Modifier Modifier = Modifier.None) : IComparer<Property>
{
    // Ordering: https://stackoverflow.com/questions/150479/order-of-items-in-classes-fields-properties-constructors-methods
    public int Compare(Property a, Property b)
        => new ModifierComparer()
            .Compare(Modifier.SetModifiers(a.Modifier), Modifier.SetModifiers(b.Modifier));
}
