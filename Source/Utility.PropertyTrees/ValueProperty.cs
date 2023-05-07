using System.ComponentModel;
using System.Globalization;
using Utility.Helpers;
using Utility.Infrastructure.Common;
using Utility.Models;

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

        public virtual PropertyDescriptor Descriptor { get; set; }

        public override Type PropertyType => Descriptor.PropertyType;

        public override object Content => Name;


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

        public override bool OnNext(object obj)
        {
            if (obj is PropertyChange { Key: Key { Guid: var guid }, NewValue: var newValue } valueChange && guid == this.Guid)
            {
                if (!TryChangeType(newValue, PropertyType, CultureInfo.CurrentCulture, out object changedValue))
                {
                    throw new ArgumentException("Cannot convert value {" + newValue + "} to type '" + PropertyType.FullName + "'.");
                }

                Descriptor?.SetValue(Data, newValue);
            }
            // property change dealt with by base
            return base.OnNext(obj);
        }
    }
}