using System.ComponentModel;
using System.Globalization;
using Utility.Conversions;
using Utility.Helpers;

namespace Utility.PropertyTrees
{
    public class ValueProperty : PropertyBase
    {
        public ValueProperty(Guid guid) : base(guid)
        {
        }

        public override string Name => Descriptor.Name;
        public string DisplayName => Descriptor.DisplayName;
        public override bool IsReadOnly => Descriptor.IsReadOnly;

        public virtual string? Category => string.IsNullOrWhiteSpace(Descriptor.Category) ||
                Descriptor.Category.EqualsIgnoreCase(CategoryAttribute.Default.Category)
                    ? null
                    : Descriptor.Category;

        public virtual TypeConverter Converter => Descriptor.Converter;

        public virtual PropertyDescriptor Descriptor { get; set; }

        public override Type PropertyType => Descriptor.PropertyType;

        public override object Content => Name;

        protected override async Task<bool> RefreshAsync()
        {
            if ((PropertyType.IsValueType || PropertyType == typeof(string)) != true)
                return await base.RefreshAsync();

            return await Task.FromResult(true);
        }

        public override object? Value
        {
            get
            {
                return GetProperty(PropertyType, Name) ?? Descriptor.GetValue(Data);
            }
            set
            {
                if (!TryChangeType(value, PropertyType, CultureInfo.CurrentCulture, out object changedValue))
                {
                    throw new ArgumentException("Cannot convert value {" + value + "} to type '" + PropertyType.FullName + "'.");
                }

                if (Descriptor != null)
                {
                    try
                    {
                        Descriptor.SetValue(Data, changedValue);
                        //var finalValue = Descriptor.GetValue(Data);
                        SetProperty(changedValue, PropertyType, Name);
                    }
                    catch (Exception e)
                    {
                        throw new ArgumentException("Cannot set value {" + value + "} to object.", e);
                    }
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}