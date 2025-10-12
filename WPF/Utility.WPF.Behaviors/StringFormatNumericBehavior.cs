using Microsoft.Xaml.Behaviors;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Utility.WPF.Helpers;

namespace Utility.WPF.Behaviors
{
    public enum NumericFormatType
    {
        Money,
        Percentage
    }

    public class StringFormatNumericBehavior : Behavior<TextBox>
    {
        public static readonly DependencyProperty NumberProperty =
            DependencyProperty.Register(
                "Number",
                typeof(decimal),
                typeof(StringFormatNumericBehavior),
                new FrameworkPropertyMetadata(0M, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// For Money: stored as actual value (e.g., 12.34 = $12.34)
        /// For Percentage: stored as fraction (e.g., 0.25 = 25%)
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
                typeof(StringFormatNumericBehavior),
                new FrameworkPropertyMetadata("C", StringFormatPropertyChanged));

        public string StringFormat
        {
            get => (string)GetValue(StringFormatProperty);
            set => SetValue(StringFormatProperty, value);
        }

        public static readonly DependencyProperty FormatTypeProperty =
            DependencyProperty.Register(
                "FormatType",
                typeof(NumericFormatType),
                typeof(StringFormatNumericBehavior),
                new FrameworkPropertyMetadata(NumericFormatType.Money, FormatTypePropertyChanged));

        public NumericFormatType FormatType
        {
            get => (NumericFormatType)GetValue(FormatTypeProperty);
            set => SetValue(FormatTypeProperty, value);
        }

        private static void StringFormatPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var behavior = obj as StringFormatNumericBehavior;
            behavior?.SetupBinding();
        }

        private static void FormatTypePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var behavior = obj as StringFormatNumericBehavior;
            behavior?.SetupBinding();
        }

        private void SetupBinding()
        {
            if (AssociatedObject == null)
                return;

            var textBinding = new Binding("Number")
            {
                Source = this,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                StringFormat = StringFormat,
                ConverterCulture = new CultureInfo("en-GB")
            };

            if (FormatType == NumericFormatType.Percentage)
            {
                textBinding.Converter = new PercentageConverter();
            }

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

            SetupBinding();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;

            // For money format with negative numbers, keep cursor before the closing parenthesis
            if (FormatType == NumericFormatType.Money && Number < 0 && StringFormat == "C")
            {
                tb.CaretIndex = tb.Text.Length - 1;
            }
            else
            {
                tb.CaretIndex = tb.Text.Length;
            }
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

                if (FormatType == NumericFormatType.Money)
                {
                    Number = Number * 10M + digit / 100M;
                }
                else // Percentage
                {
                    // Shift existing number left one digit in percentage "decimal space"
                    // Example: 0.12 (12%) → type 3 → 0.123 (12.3%)
                    var currentDigits = (int)(Number * 10000); // keep 4 digits of precision
                    currentDigits = currentDigits * 10 + (int)digit;
                    Number = currentDigits / 10000M;
                }
            }
            else if (e.Key == Key.Back)
            {
                e.Handled = true;

                if (FormatType == NumericFormatType.Money)
                {
                    Number = (Number - Number % 0.1M) / 10M;
                }
                else // Percentage
                {
                    var currentDigits = (int)(Number * 10000);
                    currentDigits = currentDigits / 10;
                    Number = currentDigits / 10000M;
                }
            }
            else if (e.Key == Key.Delete)
            {
                e.Handled = true;
                Number = 0M;
            }
            else if ((e.Key == Key.Subtract || e.Key == Key.OemMinus) && FormatType == NumericFormatType.Money)
            {
                e.Handled = true;
                Number *= -1;
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

        private class PercentageConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is decimal dec)
                {
                    // Convert fraction to percentage for display
                    return (dec).ToString("0.##") + " %";
                }
                return value;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is string s && s.EndsWith("%"))
                {
                    if (decimal.TryParse(s.Replace("%", "").Trim(), out var num))
                        return num;
                }
                return 0M;
            }
        }
    }


}