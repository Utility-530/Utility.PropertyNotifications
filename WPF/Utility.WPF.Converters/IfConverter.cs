using DryIoc.ImTools;
using LambdaConverters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Utility.Enums;

namespace Utility.WPF.Converters
{

    public enum AnyAll
    {

    }

    public class IfConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int? iQ = default;
            int? cQ = default;
            object comparison = parameter ?? true;
            Quantifier _quantifier = Quantifier.All;
            IValueConverter? _converter = null;

            if (values.SingleOrDefault(a => a is Quantifier) is Quantifier quantifier)
            {
                iQ = values.IndexOf(quantifier);
                _quantifier = quantifier;

            }

            if (values.SingleOrDefault(a => a is IValueConverter) is IValueConverter converter)
            {
                cQ = values.IndexOf(converter);
                _converter = converter;
            }

            List<object> list = new List<object>();
            for (int i = 0; i < values.Length; i++)
            {
                if (iQ.HasValue && iQ.Value == i)
                {
                    continue;
                }
                if (cQ.HasValue && cQ.Value == i)
                {
                    continue;
                }
                list.Add(values[i]);
            }

            var result = quantify(_quantifier);

            return _converter?.Convert(result, targetType, parameter, culture) ?? result;

            object quantify(Quantifier quantifier)
            {
                switch (quantifier)
                {
                    case Quantifier.None:
                        return list.All(a => a.Equals(comparison)) == false;

                    case Quantifier.All:
                        return list.All(a => a.Equals(comparison)) == true;


                    case Quantifier.Any:
                        return list.Any(a => a.Equals(comparison));

                    case Quantifier.One:
                        return list.Single(a => a.Equals(comparison));

                    case Quantifier.First:
                        return list.First(a => a.Equals(comparison));

                    case Quantifier.Last:
                        return list.Last(a => a.Equals(comparison));

                    case Quantifier.Some:
                        //return list.All(a => a == comparison) == false;
                        throw new NotImplementedException();

                    case Quantifier.Few:
                        //return list.All(a => a == comparison) == false;
                        throw new NotImplementedException();


                    case Quantifier.Many:
                        //return list.All(a => a == comparison) == false;
                        throw new NotImplementedException();


                    case Quantifier.Most:
                        //return list.All(a => a == comparison) == false;
                        throw new NotImplementedException();


                    case Quantifier.Several:
                        //return list.All(a => a == comparison) == false;
                        throw new NotImplementedException();

                    default:
                        throw new NotImplementedException();
                }
            }

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static IfConverter Instance { get; } = new();
    }
}
