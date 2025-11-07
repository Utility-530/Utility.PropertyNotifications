using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace Utility.WPF.Markup
{
    [MarkupExtensionReturnType(typeof(object))]
    public class ArrayExtension : MarkupExtension
    {
        public ArrayExtension()
        {
            Item = new Binding();
        }

        public ArrayExtension(object item)
        {
            Item = item;
        }

        public ArrayExtension(BindingBase binding)
        {
            Item = binding;
        }

        public object Item { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Item == null)
                throw new Exception("SDvf 2322");

            if (Item is Binding binding)
            {
                var newBinding = new Binding
                {
                    Path = binding.Path,

                    Mode = binding?.Mode ?? BindingMode.OneWay,
                    Converter = new SingleToArrayConverter()
                };

                if (binding.ElementName != null)
                    newBinding.ElementName = binding.ElementName;
                else if (binding.RelativeSource != null)
                    newBinding.RelativeSource = binding.RelativeSource;
                else if (binding.Source != null)
                    newBinding.Source = binding.Source;

                return newBinding.ProvideValue(serviceProvider);
            }

            // Create an array of the exact runtime type of the item
            var itemType = Item.GetType();
            var array = Array.CreateInstance(itemType, 1);
            array.SetValue(Item, 0);
            return array;
        }

        private class SingleToArrayConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                if (value == null)
                    return Array.Empty<object>();

                var t = value.GetType();
                var arr = Array.CreateInstance(t, 1);
                arr.SetValue(value, 0);
                return arr;
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
                => throw new NotSupportedException();
        }
    }
}