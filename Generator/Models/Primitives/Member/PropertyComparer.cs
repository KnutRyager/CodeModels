using System.Collections.Generic;

namespace CodeAnalyzation.Models
{
    public class PropertyComparer : IComparer<Property>
    {
        // Ordering: https://stackoverflow.com/questions/150479/order-of-items-in-classes-fields-properties-constructors-methods
        public int Compare(Property a, Property b) => Compare(a.Modifier, b.Modifier);
        public int Compare(Modifier a, Modifier b)
        {
            var constComparison = (b & Modifier.Const) - (a & Modifier.Const);
            if (constComparison is not 0) return constComparison;
            var fieldComparison = a.IsField() ^ b.IsField();
            if (fieldComparison is true) return a.IsField() ? -1 : 1;
            var enumComparison = (b & Modifier.Enum) - (a & Modifier.Enum);
            if (enumComparison is not 0) return enumComparison;
            var publicComparison = (b & Modifier.Public) - (a & Modifier.Public);
            if (publicComparison is not 0) return publicComparison;
            var internalComparison = (b & Modifier.Interface) - (a & Modifier.Interface);
            if (internalComparison is not 0) return internalComparison;
            var protectedComparison = (b & Modifier.Protected) - (a & Modifier.Protected);
            if (protectedComparison is not 0) return protectedComparison;
            var privateComparison = (b & Modifier.Private) - (a & Modifier.Private);
            if (privateComparison is not 0) return privateComparison;
            var staticComparison = (b & Modifier.Static) - (a & Modifier.Static);
            if (staticComparison is not 0) return staticComparison;
            var readonlyComparison = (b & Modifier.Readonly) - (a & Modifier.Readonly);
            if (readonlyComparison is not 0) return readonlyComparison;
            return 0;
        }
    }
}
