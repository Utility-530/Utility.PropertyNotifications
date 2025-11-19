using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace Tonic.UI
{
    /// <summary>
    /// Return all enums value for a given enum type or a binding to a property with an enum Type
    /// Ussage: 
    /// </summary>
    public class EnumSource : MarkupExtension
    {
        private readonly Type _type;
        private readonly Binding _binding;

        [ThreadStatic]
        static Dictionary<Type, object[]> cache = new Dictionary<Type, object[]>();

        /// <summary>
        /// True if friendly names should be used. If used, the EnumBinding should be used instead of the WPF binding in order to convert back from the internal representation
        /// </summary>
        public bool Friendly { get; set; } = true;

        public EnumSource(object value)
        {
            if (value is Type)
            {
                var type = (Type)value;
                if (!type.IsEnum)
                    throw new ArgumentException("EnumToItemsSource requires that the given type is a non-nullable Enum");
                _type = type;
            }
            else if (value is Binding binding)
            {
                _binding = binding;
            }
            else if (value is string str)
            {
                _binding = new Binding(str)
                {
                    Mode = BindingMode.OneWay
                };
            }
            else
                throw new ArgumentException("Value must be an Enum type, a Binding or an string");
        }


        public static readonly DependencyProperty BindingProperty =
     DependencyProperty.RegisterAttached(
         "Binding",
         typeof(Type),
         typeof(EnumSource),
         new PropertyMetadata());

        /// <summary>
        /// <a href="https://stackoverflow.com/questions/944087/get-value-from-datacontext-to-markupextension"></a>
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (!(serviceProvider is IProvideValueTarget pvt))
            {
                return null;
            }
            if (!(pvt.TargetObject is FrameworkElement frameworkElement))
            {
                return this;
            }

            //If the type is unknown;
            if (_type == null)
            {
                //provider of target object and it's property
                var targetProvider = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
                var target = (FrameworkElement)targetProvider.TargetObject;

                //Extract indirectly the property type and the enum info using the GetTypeEnumConverter:
                var conv = new GetTypeEnumConverter
                {
                    UseDescription = Friendly
                };

                if (_binding.Path != null)
                {
                    var binding = new Binding(_binding.Path.Path)
                    {
                        Mode = BindingMode.TwoWay,
                        Converter = conv
                    };

                    return conv.Expression = (BindingExpression)binding.ProvideValue(serviceProvider);
                }
                return null;
            }
            else
            {
                //The type is already known, extract enum info:
                if (Friendly)
                    return GetEnumValues(_type).Select(x => EnumValue.Create(x));
                else
                    return GetEnumValues(_type);
            }
        }

        /// <summary>
        /// Get the Enum values for a given Enum type
        /// </summary>
        /// <param name="Type">Enum or nullable Enum type</param>
        /// <returns></returns>
        public static IEnumerable<object> GetEnumValues(Type Type)
        {
            //Extract nullable argument:
            if (Type.IsGenericType && Type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                Type = Type.GetGenericArguments()[0];
            }

            object[] result;
            if (!cache.TryGetValue(Type, out result))
            {
                result = Enum.GetValues(Type).Cast<object>().ToArray();
                cache.Add(Type, result);
            }

            return result;
        }
    }
}