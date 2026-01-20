using System.Globalization;
using System.Windows.Data;
using Microsoft.CodeAnalysis;

namespace WPF.ComboBoxes.Roslyn
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
            if (obj is IntelliSenseResult { Symbol.Item: ISymbol typeSymbol } specifier)
            {
                return typeSymbol?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
            }
            else if (obj is ISymbol symbol)
            {
                return symbol?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
            }
            else
            {
                throw new Exception("dsdf e ");
            }
        }
        public static IReadOnlyList<TextSpan> ToSpans(object obj)
        {
            if (obj is IntelliSenseResult { Symbol.Item: ISymbol typeSymbol } specifier)
            {
                return specifier.Match.MatchedSpans;
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