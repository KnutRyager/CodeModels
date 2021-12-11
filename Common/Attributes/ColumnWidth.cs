using System;

namespace Common.Attributes
{
    [AttributeUsage(System.AttributeTargets.Property)
    ]
    public class ColumnWidthAttribute : Attribute
    {
        public int Width;

        public ColumnWidthAttribute(int width)
        {
            Width = width;
        }
    }
}