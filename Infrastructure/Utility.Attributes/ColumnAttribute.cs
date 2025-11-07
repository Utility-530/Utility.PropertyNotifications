using System;
using Utility.Enums;

namespace Utility.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute(string? displayName = null, bool ignore = false, Sort sort = Sort.None, double width = 150, double minWidth = default, double maxWidth = default, DimensionUnitType dimensionUnitType = DimensionUnitType.Auto) : Attribute
    {
        public string? DisplayName { get; } = displayName;
        public bool Ignore { get; } = ignore;
        public Sort Sort { get; } = sort;
        public double Width { get; } = width;
        public double MinWidth { get; } = minWidth;
        public double MaxWidth { get; } = maxWidth;
        public DimensionUnitType UnitType { get; } = dimensionUnitType;
    }
}