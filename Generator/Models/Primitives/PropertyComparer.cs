using System.Collections.Generic;

namespace CodeAnalyzation.Models
{
    public class PropertyComparer : IComparer<Property>
    {
        public int Compare(Property a, Property b) => a.PropertyType - b.PropertyType;
    }
}
