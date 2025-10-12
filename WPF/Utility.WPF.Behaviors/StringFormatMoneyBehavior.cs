using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Utility.WPF.Helpers;

namespace Utility.WPF.Behaviors
{
    /// <summary>
    /// <a href="https://github.com/abdalkhalik/CurrencyTextBox"></a>
    /// <a href="https://github.com/mtusk/wpf-currency-textbox"></a>
    /// </summary>
    public class StringFormatMoneyBehavior : Behavior<TextBox>
    {
        public static readonly DependencyProperty NumberProperty = DependencyProperty.Register(
            "Number",
            typeof(decimal),
            typeof(StringFormatMoneyBehavior),
            new FrameworkPropertyMetadata(0M, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public decimal Number
        {
            get => (decimal)GetValue(NumberProperty);
            set => SetValue(NumberProperty, value);
        }

        public static readonly DependencyProperty StringFormatProperty = DependencyProperty.Register(
            "StringFormat",
            typeof(string),
            typeof(StringFormatMoneyBehavior),
            new FrameworkPropertyMetadata("C", StringFormatPropertyChanged));

        public string StringFormat
        {
            get => (string)GetValue(StringFormatProperty);
            set => SetValue(StringFormatProperty, value);
        }

        private static void StringFormatPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var behavior = obj as StringFormatMoneyBehavior;
            var textBox = behavior.AssociatedObject;
            if (textBox != null)
            {
                var textBinding = new Binding("Number")
                {
                    Source = behavior,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    StringFormat = e.NewValue as string
                };

                BindingOperations.SetBinding(textBox, TextBox.TextProperty, textBinding);
            }
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
            var textBinding = new Binding("Number")
            {
                Source = this,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                StringFormat = StringFormat
            };

            BindingOperations.SetBinding(textBox, TextBox.TextProperty, textBinding);

        }




        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;
            if (Number < 0 && tb.GetBindingExpression(TextBox.TextProperty).ParentBinding.StringFormat == "C")
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
            var textBox = sender as TextBox;

            if (e.Key.IsNumeric())
            {
                e.Handled = true;
                Number = Number * 10M + e.Key.ExtractDigit() / 100M;
            }
            else if (e.Key == Key.Back)
            {
                e.Handled = true;
                Number = (Number - Number % 0.1M) / 10M;
            }
            else if (e.Key == Key.Delete)
            {
                e.Handled = true;
                Number = 0M;
            }
            else if (e.Key == Key.Subtract || e.Key == Key.OemMinus)
            {
                e.Handled = true;
                Number *= -1;
            }
            else if (KeyHelpers.IsIgnoredKey(e.Key))
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

    internal static class KeyHelpers
    {
        public static bool IsIgnoredKey(Key key)
        {
            return key == Key.Up ||
                key == Key.Down ||
                key == Key.Tab ||
                key == Key.Enter;
        }
    }
    

}
