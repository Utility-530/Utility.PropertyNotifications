using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace CurrencyTextBox
{
    /// <summary>
    /// <a href="https://github.com/abdalkhalik/CurrencyTextBox"></a>
    /// <a href="https://github.com/mtusk/wpf-currency-textbox"></a>
    /// </summary>
    public class CurrencyTextBoxBehavior : Behavior<TextBox>
    {
        public static readonly DependencyProperty NumberProperty = DependencyProperty.Register(
            "Number",
            typeof(decimal),
            typeof(CurrencyTextBoxBehavior),
            new FrameworkPropertyMetadata(0M, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public decimal Number
        {
            get => (decimal)GetValue(NumberProperty);
            set => SetValue(NumberProperty, value);
        }

        public static readonly DependencyProperty StringFormatProperty = DependencyProperty.Register(
            "StringFormat",
            typeof(string),
            typeof(CurrencyTextBoxBehavior),
            new FrameworkPropertyMetadata("C", StringFormatPropertyChanged));

        public string StringFormat
        {
            get => (string)GetValue(StringFormatProperty);
            set => SetValue(StringFormatProperty, value);
        }

        private static void StringFormatPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var behavior = obj as CurrencyTextBoxBehavior;
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
                StringFormat = this.StringFormat
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

            if (IsNumericKey(e.Key))
            {
                e.Handled = true;
                Number = (Number * 10M) + (GetDigitFromKey(e.Key) / 100M);
            }
            else if (e.Key == Key.Back)
            {
                e.Handled = true;
                Number = (Number - (Number % 0.1M)) / 10M;
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
            else if (IsIgnoredKey(e.Key))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private decimal GetDigitFromKey(Key key)
        {
            switch (key)
            {
                case Key.D0:
                case Key.NumPad0: return 0M;
                case Key.D1:
                case Key.NumPad1: return 1M;
                case Key.D2:
                case Key.NumPad2: return 2M;
                case Key.D3:
                case Key.NumPad3: return 3M;
                case Key.D4:
                case Key.NumPad4: return 4M;
                case Key.D5:
                case Key.NumPad5: return 5M;
                case Key.D6:
                case Key.NumPad6: return 6M;
                case Key.D7:
                case Key.NumPad7: return 7M;
                case Key.D8:
                case Key.NumPad8: return 8M;
                case Key.D9:
                case Key.NumPad9: return 9M;
                default: throw new ArgumentOutOfRangeException("Invalid key: " + key.ToString());
            }
        }

        private bool IsNumericKey(Key key)
        {
            return key == Key.D0 ||
                key == Key.D1 ||
                key == Key.D2 ||
                key == Key.D3 ||
                key == Key.D4 ||
                key == Key.D5 ||
                key == Key.D6 ||
                key == Key.D7 ||
                key == Key.D8 ||
                key == Key.D9 ||
                key == Key.NumPad0 ||
                key == Key.NumPad1 ||
                key == Key.NumPad2 ||
                key == Key.NumPad3 ||
                key == Key.NumPad4 ||
                key == Key.NumPad5 ||
                key == Key.NumPad6 ||
                key == Key.NumPad7 ||
                key == Key.NumPad8 ||
                key == Key.NumPad9;
        }

        private bool IsBackspaceKey(Key key)
        {
            return key == Key.Back;
        }

        private bool IsIgnoredKey(Key key)
        {
            return key == Key.Up ||
                key == Key.Down ||
                key == Key.Tab ||
                key == Key.Enter;
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

}
