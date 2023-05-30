using System.ComponentModel;
using Utility.Helpers;

namespace Utility.PropertyTrees
{
    public class ReferenceProperty : PropertyBase
    {
        public ReferenceProperty(Guid guid) : base(guid)
        {
        }

        public override string Name => Descriptor?.Name ?? "Descriptor not set";
        public string DisplayName => Descriptor.DisplayName;
        public override bool IsReadOnly => Descriptor.IsReadOnly;
        public bool IsFlagsEnum => PropertyType.IsFlagsEnum();

        public virtual string? Category => string.IsNullOrWhiteSpace(Descriptor.Category) ||
                Descriptor.Category.EqualsIgnoreCase(CategoryAttribute.Default.Category)
                    ? null
                    : Descriptor.Category;

        public virtual TypeConverter Converter => Descriptor.Converter;

        public override Type PropertyType => Descriptor.PropertyType;

        public override object Content => Name;

        public override object? Value
        {
            get
            {
                var property = Data;
                return property;
            }
            set => throw new Exception("aa 4 43321``");
        }

        public override bool HasChildren => PropertyType != typeof(string);

        public override string ToString()
        {
            return Name;
        }



    }
}