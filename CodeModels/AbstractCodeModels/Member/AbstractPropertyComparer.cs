﻿using System.Collections.Generic;
using CodeModels.Models;

namespace CodeModels.AbstractCodeModels.Member;

public record AbstractPropertyComparer(Modifier Modifier = Modifier.None) : IComparer<AbstractProperty>
{
    // Ordering: https://stackoverflow.com/questions/150479/order-of-items-in-classes-fields-properties-constructors-methods
    public int Compare(AbstractProperty a, AbstractProperty b)
        => new ModifierComparer()
            .Compare(Modifier.SetModifiers(a.Modifier), Modifier.SetModifiers(b.Modifier));
}
