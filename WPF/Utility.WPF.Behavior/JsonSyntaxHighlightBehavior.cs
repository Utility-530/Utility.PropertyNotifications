using DynamicData;
using Evan.Wpf;
using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace Utility.WPF.Behavior
{
    using Helper;

    /// <summary>
    /// based on
    /// <a href="https://github.com/daeyeol/wpf-syntax-highlighting/tree/master/SyntaxHighlighting"></a>
    /// </summary>
    public class JsonSyntaxHighlightBehavior : Behavior<RichTextBox>
    {
        private const string Pattern = "(\"(\\\\u[a-zA-Z0-9]{4}|\\\\[^u]|[^\\\\\"])*\"(\\s*:)?|\\b(true|false|null)\\b|-?\\d+(?:\\.\\d*)?(?:[eE][+\\-]?\\d+)?)";

        public static readonly DependencyProperty JsonProperty = DependencyHelper.Register<string>(new(PropertyChanged));
        public static readonly DependencyProperty LineHeightProperty = DependencyHelper.Register<double>(new(5d, LineHeightPropertyChanged));
        public static readonly DependencyProperty KeyColorProperty = DependencyHelper.Register<SolidColorBrush>(new("#FF7CDCFE".ToColor(), PropertyChanged));
        public static readonly DependencyProperty StringColorProperty = DependencyHelper.Register<SolidColorBrush>(new("#FFC3703C".ToColor(), PropertyChanged));
        public static readonly DependencyProperty NumberColorProperty = DependencyHelper.Register<SolidColorBrush>(new("#FFB5CEA8".ToColor(), PropertyChanged));
        public static readonly DependencyProperty BoooleanColorProperty = DependencyHelper.Register<SolidColorBrush>(new("#FF569CCA".ToColor(), PropertyChanged));

        #region properties

        public string Json
        {
            get { return (string)GetValue(JsonProperty); }
            set { SetValue(JsonProperty, value); }
        }

        public double LineHeight
        {
            get { return (double)GetValue(LineHeightProperty); }
            set { SetValue(LineHeightProperty, value); }
        }

        public SolidColorBrush KeyColor
        {
            get { return (SolidColorBrush)GetValue(KeyColorProperty); }
            set { SetValue(KeyColorProperty, value); }
        }

        public SolidColorBrush StringColor
        {
            get { return (SolidColorBrush)GetValue(StringColorProperty); }
            set { SetValue(StringColorProperty, value); }
        }

        public SolidColorBrush NumberColor
        {
            get { return (SolidColorBrush)GetValue(NumberColorProperty); }
            set { SetValue(NumberColorProperty, value); }
        }

        public SolidColorBrush BoooleanColor
        {
            get { return (SolidColorBrush)GetValue(BoooleanColorProperty); }
            set { SetValue(BoooleanColorProperty, value); }
        }

        #endregion properties

        protected override void OnAttached()
        {
            AssociatedObject.Foreground = Brushes.White;
            AssociatedObject.Background = "#FF1E1E1E".ToColor();
            base.OnAttached();
        }

        public static void PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is JsonSyntaxHighlightBehavior { AssociatedObject.Document: { } document } box)
                box.Update(document);
        }

        private static void LineHeightPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is JsonSyntaxHighlightBehavior { AssociatedObject.Document: { } document })
                document.LineHeight = (double)e.NewValue;
        }

        private void Update(FlowDocument document)
        {
            document.Blocks.Clear();

            if (string.IsNullOrWhiteSpace(Json))
            {
                return;
            }

            FlowDocument flowDocument = new()
            {
                LineHeight = LineHeight
            };

            Binding binding = new()
            {
                Source = this,
                Path = new PropertyPath("ActualWidth"),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            };

            BindingOperations.SetBinding(flowDocument, FlowDocument.PageWidthProperty, binding);

            document = flowDocument;

            var lines = Json.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            var pattern = Pattern;
            var keyRanges = new List<TextRange>();
            var valueRanges = new List<TextRange>();

            foreach (var line in lines)
            {
                Paragraph paragraph = new Paragraph(new Run(line));
                flowDocument.Blocks.Add(paragraph);
            }

            var position = document.ContentStart;

            while (position != null)
            {
                if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    var text = position.GetTextInRun(LogicalDirection.Forward);
                    var matches = Regex.Matches(text, pattern)
                        .Cast<Match>().ToList();

                    foreach (var match in matches)
                    {
                        if (string.IsNullOrWhiteSpace(match.Value))
                        {
                            continue;
                        }

                        if (match.Value.Last() == ':')
                        {
                            var range = CreateTextRange(position, match.Index, match.Length - 1);
                            keyRanges.Add(range);
                        }
                        else
                        {
                            var range = CreateTextRange(position, match.Index, match.Length);
                            valueRanges.Add(range);
                        }
                    }
                }

                position = position.GetNextContextPosition(LogicalDirection.Forward);
            }

            foreach (var range in keyRanges)
            {
                range.ApplyPropertyValue(TextElement.ForegroundProperty, KeyColor);
            }

            foreach (var range in valueRanges)
            {
                if (Regex.IsMatch(range.Text, "^[0-9]*$"))
                {
                    range.ApplyPropertyValue(TextElement.ForegroundProperty, NumberColor);
                }
                else if (Regex.IsMatch(range.Text, "false|true"))
                {
                    range.ApplyPropertyValue(TextElement.ForegroundProperty, BoooleanColor);
                }
                else
                {
                    range.ApplyPropertyValue(TextElement.ForegroundProperty, StringColor);
                }
            }

            AssociatedObject.Document = document;

            static TextRange CreateTextRange(TextPointer pointer, int index, int length)
            {
                var startPointer = pointer.GetPositionAtOffset(index);
                var endPointer = startPointer.GetPositionAtOffset(length);

                return new TextRange(startPointer, endPointer);
            }
        }
    }

    namespace Helper
    {
        internal static class SolidBrushHelper
        {
            public static SolidColorBrush ToColor(this string colorString)
            {
                var color = (Color)ColorConverter.ConvertFromString(colorString);
                return new SolidColorBrush(color);
            }
        }
    }
}