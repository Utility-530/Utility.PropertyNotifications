using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using Newtonsoft.Json;

namespace Utility.Meta
{
    public class RootDescriptor : PropertyDescriptor
    {
        private object component;
        public const string DefaultName = "root";

        public RootDescriptor(Type type, Type? parentType = null, string? name = null) : base(name ?? type.Name ?? DefaultName, null)
        {
            if (type == null)
            {
            }
            if (parentType == null)
            {
            }
            PropertyType = type;
            ComponentType = parentType;
            Converter = new RootTypeConverter();
        }

        public object Item => component;

        public override Type ComponentType { get; }

        public override bool IsReadOnly { get; } = false;

        public override Type PropertyType { get; }

        [JsonIgnore]
        public override TypeConverter Converter { get; }

        public override bool CanResetValue(object component)
        {
            throw new NotImplementedException();
        }

        public override object? GetValue(object? component)
        {
            if (component != null)
            {
                return this.component = component;
            }
            else
            {
                return this.component ??= instance(PropertyType);
            }
            object instance(Type type)
            {
                if (type == typeof(string))
                {
                    return string.Empty;
                }
                if (type == typeof(Assembly))
                {
                    return null;
                }
                return Activator.CreateInstance(type);
            }
        }

        public override void ResetValue(object component)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object? component, object? value)
        {
            this.component = value;
        }

        public override bool ShouldSerializeValue(object component)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            try
            {
                if (PropertyType == null)
                {
                    return -1;
                }
                var hc = base.GetHashCode();
                return hc;
            }
            catch (Exception ex)
            {
                return PropertyType?.GetHashCode() ?? -5;
            }
        }
    }

    public class RootTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            return base.CreateInstance(context, propertyValues);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        {
            return base.GetCreateInstanceSupported(context);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return base.GetProperties(context, value, attributes);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return base.GetPropertiesSupported(context);
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return base.GetStandardValues(context);
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return base.GetStandardValuesExclusive(context);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return base.GetStandardValuesSupported(context);
        }

        public override bool IsValid(ITypeDescriptorContext context, object value)
        {
            return base.IsValid(context, value);
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}