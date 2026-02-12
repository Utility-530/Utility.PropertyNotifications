using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using Microsoft.CodeAnalysis;
using Utility.PatternMatchings;
using Utility.Roslyn;

namespace Utility.WPF.ComboBoxes.Roslyn
{
    public class SymbolStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ToString(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        public static string ToString(object obj)
        {
            if (obj is Result { } specifier)
            {
                if (specifier.Symbol.Item is ISymbol typeSymbol)
                    return typeSymbol?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
                else if (specifier.Symbol.Item is PropertyInfo propertyInfo)
                    return propertyInfo.Name;
                else
                    throw new Exception("DFS 3f s3 f");
            }
            else if (obj is ISymbol symbol)
            {
                return symbol?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
            }
            else
            {
                //throw new Exception("dsdf e ");
                return null;
            }
        }
        public static IEnumerable<TextSpan> ToSpans(object obj)
        {
            if (obj is Result { } specifier)
            {
                return specifier?.Match?.MatchedSpans ?? Array.Empty<TextSpan>();
            }
            else if (obj is ISymbol symbol)
            {
                return new[] { new TextSpan(0, ToString(symbol).Length) { } };
            }
            else
            {
                throw new Exception("dsdf e ");
            }
        }
    }
}