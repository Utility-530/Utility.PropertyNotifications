using Microsoft.Xaml.Behaviors;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Utility.WPF.Behaviors
{
    public class StringFormatPinBehavior : Behavior<TextBox>
    {
        public int MaxDigits { get; set; } = 4;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewTextInput += OnPreviewTextInput;
            AssociatedObject.PreviewKeyDown += OnPreviewKeyDown;

            // Disable paste command
            CommandManager.AddPreviewExecutedHandler(AssociatedObject, OnPreviewExecuted);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewTextInput -= OnPreviewTextInput;
            AssociatedObject.PreviewKeyDown -= OnPreviewKeyDown;

            CommandManager.RemovePreviewExecutedHandler(AssociatedObject, OnPreviewExecuted);
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = true;
            var textBox = AssociatedObject;

            if (!Regex.IsMatch(e.Text, "^[0-9]$"))
                return;

            var digits = GetDigits(textBox.Text);

            if (digits.Length >= MaxDigits)
                return;

            digits += e.Text;
            textBox.Text = FormatWithDots(digits);
            textBox.CaretIndex = textBox.Text.Length;
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var textBox = AssociatedObject;
            var digits = GetDigits(textBox.Text);

            if (e.Key == Key.Back)
            {
                e.Handled = true;
                if (digits.Length > 0)
                {
                    digits = digits.Substring(0, digits.Length - 1);
                    textBox.Text = FormatWithDots(digits);
                    textBox.CaretIndex = textBox.Text.Length;
                }
            }
        }

        private void OnPreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            // Disable paste
            if (e.Command == ApplicationCommands.Paste)
                e.Handled = true;
        }

        private static string GetDigits(string input)
        {
            return new string(input.Where(char.IsDigit).ToArray());
        }

        private string FormatWithDots(string digits)
        {
            return string.Join(".", digits.ToCharArray());
        }

        public string GetRawPin() => GetDigits(AssociatedObject.Text);
    }
}
