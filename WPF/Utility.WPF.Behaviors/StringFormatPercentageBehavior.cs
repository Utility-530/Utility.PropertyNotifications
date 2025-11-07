using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using Utility.WPF.Helpers;

namespace Utility.WPF.Behaviors
{
    public class StringFormatPercentageBehavior : Behavior<TextBox>
    {
        public static readonly DependencyProperty NumberProperty =
            DependencyProperty.Register(
                "Number",
                typeof(decimal),
                typeof(StringFormatPercentageBehavior),
                new FrameworkPropertyMetadata(0M, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Value stored as fraction (e.g., 0.25 = 25%)
        /// </summary>
        public decimal Number
        {
            get => (decimal)GetValue(NumberProperty);
            set => SetValue(NumberProperty, value);
        }

        public static readonly DependencyProperty StringFormatProperty =
            DependencyProperty.Register(
                "StringFormat",
                typeof(string),
                typeof(StringFormatPercentageBehavior),
                new FrameworkPropertyMetadata(StringFormatPropertyChanged));

        public string StringFormat
        {
            get => (string)GetValue(StringFormatProperty);
            set => SetValue(StringFormatProperty, value);
        }

        private static void StringFormatPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var behavior = obj as StringFormatPercentageBehavior;
            behavior.setBinding(e.NewValue as string);
        }

        private void setBinding(string stringFormat)
        {
            if (AssociatedObject == null)
                return;
            var textBinding = new Binding("Number")
            {
                Source = this,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                StringFormat = stringFormat,
                Converter = new PercentageConverter()
            };

            BindingOperations.SetBinding(AssociatedObject, TextBox.TextProperty, textBinding);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            var textBox = AssociatedObject;
            textBox.TextChanged += TextBox_TextChanged;
            textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
            textBox.PreviewMouseDown += TextBox_PreviewMouseDown;
            textBox.PreviewMouseUp += TextBox_PreviewMouseUp;
            textBox.ContextMenu = null;

            // Bind Text to Number with the specified StringFormat
            setBinding(StringFormat);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;
            tb.CaretIndex = tb.Text.Length;
        }

        private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            (sender as TextBox).Focus();
        }

        private void TextBox_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            (sender as TextBox).Focus();
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.IsNumeric())
            {
                e.Handled = true;

                var digit = e.Key.ExtractDigit();

                // Shift existing number left one digit in percentage "decimal space"
                // Example: 0.12 (12%) → type 3 → 0.123 (12.3%)
                var currentDigits = (int)(Number * 10000); // keep 4 digits of precision
                currentDigits = currentDigits * 10 + (int)digit;

                Number = currentDigits / 10000M;
            }
            else if (e.Key == Key.Back)
            {
                e.Handled = true;

                var currentDigits = (int)(Number * 10000); // remove last digit
                currentDigits = currentDigits / 10;

                Number = currentDigits / 10000M;
            }
            else if (e.Key == Key.Delete)
            {
                e.Handled = true;
                Number = 0M;
            }
            else if (e.Key.IsIgnored())
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            var textBox = AssociatedObject;
            textBox.TextChanged -= TextBox_TextChanged;
            textBox.PreviewKeyDown -= TextBox_PreviewKeyDown;
            textBox.PreviewMouseDown -= TextBox_PreviewMouseDown;
            textBox.PreviewMouseUp -= TextBox_PreviewMouseUp;
        }
    }

    public class PercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal dec)
            {
                return (dec).ToString("0.##") + " %";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s && s.EndsWith("%"))
            {
                if (decimal.TryParse(s.Replace("%", "").Trim(), out var num))
                    return num / 100M;
            }
            return 0M;
        }
    }
}