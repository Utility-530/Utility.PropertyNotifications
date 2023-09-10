using System.ComponentModel;
using Utility.Helpers;
using Utility.Interfaces.NonGeneric;

namespace Utility.PropertyTrees
{
    public class ValueProperty : PropertyBase
    {
        public ValueProperty(Guid guid) : base(guid)
        {
        }

        public override string Name => Descriptor?.Name ?? $"{nameof(Descriptor)} not set";
        public string DisplayName => Descriptor.DisplayName;
        public override bool IsReadOnly => Descriptor.IsReadOnly;
        public virtual string? Category => string.IsNullOrWhiteSpace(Descriptor.Category) ||
                Descriptor.Category.EqualsIgnoreCase(CategoryAttribute.Default.Category)
                    ? null
                    : Descriptor.Category;

        public virtual TypeConverter Converter => Descriptor.Converter;

        //public override bool HasChildren => false;

        public override Type PropertyType => Descriptor.PropertyType;

        //protected object? GetValue(IEquatable name)
        //{
        //    return Descriptor?.GetValue(Data);
        //}

        //protected void SetValue(IEquatable name, object value)
        //{
        //    Descriptor.SetValue(Data, value);
        //    OnPropertyChanged(nameof(Value));
        //}
    }
}