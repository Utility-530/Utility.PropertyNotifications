using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Tonic.UI
{
    class GetTypeEnumConverter : IValueConverter
    {
        private Dictionary<object, object> cache = new Dictionary<object, object>();

        public BindingExpression Expression { get; set; }
        /// <summary>
        /// True to use Friendly names
        /// </summary>
        public bool UseDescription { get; set; }
        /// <summary>
        /// Null if the type is still unknown
        /// </summary>
        //private Type Type { get; set; }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Expression == null) return null;

            if (value == null)
                return DependencyProperty.UnsetValue;
            if (cache.ContainsKey(value) == false)
            {
                //if (Type == null)
                var Type = Expression.ResolvedSource.GetType().GetProperty(Expression.ResolvedSourcePropertyName).GetValue(Expression.ResolvedSource)?.GetType();

                if (Type == null)
                    return DependencyProperty.UnsetValue;

                //El cache no es solo por rendimiento, si no es utilizado, cada vez que se llama a este metodo,
                //aunque sean los mismos valores devuelve una instancia de un arreglo diferente, lo que ocasiona que
                //WPF actualize el valor de la propiedad, ocacionando un ciclo infinito


                if (UseDescription)
                    cache[value] = EnumSource.GetEnumValues(Type).Select(x => EnumValue.Create(x)).ToArray();
                else
                    cache[value] = EnumSource.GetEnumValues(Type);
            }
            return cache[value];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}