using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using WPF.ComboBoxes.Roslyn;

namespace WPF.ComboBoxes.Roslyn
{
    public class HighlightTextConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2 || values[0] == null || parameter == null)
                return new List<HighlightTextSegment>();

 
            string searchText = values[1] as string ?? string.Empty;

            string displayText = null;
            IReadOnlyList<TextSpan> spans = null;
            if (values[0] is IntelliSenseResult { Symbol.Item: ISymbol typeSymbol } specifier)
            {
                displayText = typeSymbol?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
                spans = specifier.Match.MatchedSpans;
            }
            else if (values[0] is ISymbol symbol)
            {
                displayText = symbol?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
                spans = new[] { new TextSpan(0, displayText.Length) {  } };
            }
            else
                throw new Exception("R R$dfg c");

            if (string.IsNullOrEmpty(displayText))
                return new List<HighlightTextSegment>();

            if (string.IsNullOrEmpty(searchText))
            {
                return new List<HighlightTextSegment>
                {
                    new HighlightTextSegment { Text = displayText, IsMatch = false }
                };
            }

            return HighlightMatches(displayText, spans);
        }

        private List<HighlightTextSegment> HighlightMatches(string fullText, IReadOnlyList<TextSpan> spans)
        {
            var segments = new List<HighlightTextSegment>();

            int currentPos = 0;

            if (spans != null)
                foreach (var span in spans)
                {
                    // Add any gap before this segment
                    if (span.Start > currentPos)
                    {
                        segments.Add(new HighlightTextSegment
                        {
                            Text = fullText.Substring(currentPos, span.Start - currentPos),
                            IsMatch = false
                        });
                    }

                    // Add the segment itself
                    segments.Add(new HighlightTextSegment
                    {
                        Text = fullText.Substring(span.Start, span.Length),
                        IsMatch = true
                    });

                    currentPos = span.Start + span.Length;
                }

            // Add any remaining text
            if (currentPos < fullText.Length)
            {
                segments.Add(new HighlightTextSegment
                {
                    Text = fullText.Substring(currentPos),
                    IsMatch = false
                });
            }

            return segments;
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}