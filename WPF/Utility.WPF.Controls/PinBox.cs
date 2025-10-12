using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Utility.WPF.Controls
{
    /// <summary>
    /// PIN Code Control for 4-digit entry
    /// </summary>
    [TemplatePart(Name = PART_PinDigit1, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_PinDigit2, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_PinDigit3, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_PinDigit4, Type = typeof(TextBox))]
    public class PinBox : TextBox
    {
        private const string PART_PinDigit1 = "PART_PinDigit1";
        private const string PART_PinDigit2 = "PART_PinDigit2";
        private const string PART_PinDigit3 = "PART_PinDigit3";
        private const string PART_PinDigit4 = "PART_PinDigit4";

        static PinBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PinBox), new FrameworkPropertyMetadata(typeof(PinBox)));
            TextProperty.AddOwner(typeof(PinBox));
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }

        public override void OnApplyTemplate()
        {
            InputMethod.SetIsInputMethodEnabled(this, false);

            base.OnApplyTemplate();

            PinDigit1 = GetTemplateChild(PART_PinDigit1) as TextBox;
            PinDigit2 = GetTemplateChild(PART_PinDigit2) as TextBox;
            PinDigit3 = GetTemplateChild(PART_PinDigit3) as TextBox;
            PinDigit4 = GetTemplateChild(PART_PinDigit4) as TextBox;

            RefreshPinDigits(this);
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            RefreshPinDigits(this);
            base.OnTextChanged(e);
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);

            if (!PinDigit1.IsKeyboardFocused && !PinDigit2.IsKeyboardFocused &&
                !PinDigit3.IsKeyboardFocused && !PinDigit4.IsKeyboardFocused)
            {
                if (!string.IsNullOrWhiteSpace(PinDigit4.Text) || !string.IsNullOrWhiteSpace(Text))
                {
                    PinDigit4.Focus();
                }
                else
                {
                    PinDigit1.Focus();
                }
            }
        }

        private static void RefreshPinDigits(PinBox pinBox)
        {
            if (!string.IsNullOrWhiteSpace(pinBox.Text) && pinBox.Text.Length <= 4)
            {
                char[] digits = pinBox.Text.ToCharArray();

                if (digits.Length > 0 && pinBox.PinDigit1 != null && pinBox.PinDigit1.Text != digits[0].ToString())
                {
                    pinBox.PinDigit1.Text = digits[0].ToString();
                }
                if (digits.Length > 1 && pinBox.PinDigit2 != null && pinBox.PinDigit2.Text != digits[1].ToString())
                {
                    pinBox.PinDigit2.Text = digits[1].ToString();
                }
                if (digits.Length > 2 && pinBox.PinDigit3 != null && pinBox.PinDigit3.Text != digits[2].ToString())
                {
                    pinBox.PinDigit3.Text = digits[2].ToString();
                }
                if (digits.Length > 3 && pinBox.PinDigit4 != null && pinBox.PinDigit4.Text != digits[3].ToString())
                {
                    pinBox.PinDigit4.Text = digits[3].ToString();
                }
            }
        }

        private TextBox pinDigit1;
        private TextBox PinDigit1
        {
            get { return pinDigit1; }
            set
            {
                if (value != null)
                {
                    InputMethod.SetIsInputMethodEnabled(value, false);
                    value.PreviewKeyDown -= PinDigit_PreviewKeyDown;
                    value.PreviewKeyDown += PinDigit_PreviewKeyDown;
                    value.PreviewTextInput -= PinDigit_PreviewTextInput;
                    value.PreviewTextInput += PinDigit_PreviewTextInput;
                    value.TextChanged -= PinDigit_TextChanged;
                    value.TextChanged += PinDigit_TextChanged;
                }
                pinDigit1 = value;
            }
        }

        private TextBox pinDigit2;
        private TextBox PinDigit2
        {
            get { return pinDigit2; }
            set
            {
                if (value != null)
                {
                    InputMethod.SetIsInputMethodEnabled(value, false);
                    value.PreviewKeyDown -= PinDigit_PreviewKeyDown;
                    value.PreviewKeyDown += PinDigit_PreviewKeyDown;
                    value.PreviewTextInput -= PinDigit_PreviewTextInput;
                    value.PreviewTextInput += PinDigit_PreviewTextInput;
                    value.TextChanged -= PinDigit_TextChanged;
                    value.TextChanged += PinDigit_TextChanged;
                }
                pinDigit2 = value;
            }
        }

        private TextBox pinDigit3;
        private TextBox PinDigit3
        {
            get { return pinDigit3; }
            set
            {
                if (value != null)
                {
                    InputMethod.SetIsInputMethodEnabled(value, false);
                    value.PreviewKeyDown -= PinDigit_PreviewKeyDown;
                    value.PreviewKeyDown += PinDigit_PreviewKeyDown;
                    value.PreviewTextInput -= PinDigit_PreviewTextInput;
                    value.PreviewTextInput += PinDigit_PreviewTextInput;
                    value.TextChanged -= PinDigit_TextChanged;
                    value.TextChanged += PinDigit_TextChanged;
                }
                pinDigit3 = value;
            }
        }

        private TextBox pinDigit4;
        private TextBox PinDigit4
        {
            get { return pinDigit4; }
            set
            {
                if (value != null)
                {
                    InputMethod.SetIsInputMethodEnabled(value, false);
                    value.PreviewKeyDown -= PinDigit_PreviewKeyDown;
                    value.PreviewKeyDown += PinDigit_PreviewKeyDown;
                    value.PreviewTextInput -= PinDigit_PreviewTextInput;
                    value.PreviewTextInput += PinDigit_PreviewTextInput;
                    value.TextChanged -= PinDigit_TextChanged;
                    value.TextChanged += PinDigit_TextChanged;
                }
                pinDigit4 = value;
            }
        }

        private void PinDigit_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Only allow single digits 0-9
            if (!int.TryParse(e.Text, out int digit) || e.Text.Length > 1)
            {
                e.Handled = true;
                return;
            }

            TextBox textBox = sender as TextBox;

            // If there's already a digit, replace it and move to next
            if (!string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = e.Text;
                if (textBox != PinDigit4)
                {
                    textBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }
                e.Handled = true;
            }
        }

        private void PinDigit_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Ensure only single digit
            if (textBox.Text.Length > 1)
            {
                textBox.Text = textBox.Text.Substring(0, 1);
                textBox.CaretIndex = 1;
            }

            // Auto-advance to next box when digit entered
            if (textBox.Text.Length == 1)
            {
                if (textBox == PinDigit1 && string.IsNullOrWhiteSpace(PinDigit2.Text))
                {
                    PinDigit2.Focus();
                }
                else if (textBox == PinDigit2 && string.IsNullOrWhiteSpace(PinDigit3.Text))
                {
                    PinDigit3.Focus();
                }
                else if (textBox == PinDigit3 && string.IsNullOrWhiteSpace(PinDigit4.Text))
                {
                    PinDigit4.Focus();
                }
            }

            // Update main Text property
            if (string.IsNullOrWhiteSpace(PinDigit1.Text) ||
                string.IsNullOrWhiteSpace(PinDigit2.Text) ||
                string.IsNullOrWhiteSpace(PinDigit3.Text) ||
                string.IsNullOrWhiteSpace(PinDigit4.Text))
            {
                this.Text = string.Empty;
            }
            else
            {
                this.Text = PinDigit1.Text + PinDigit2.Text + PinDigit3.Text + PinDigit4.Text;
            }
        }

        private void PinDigit_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            switch (e.Key)
            {
                case Key.D0:
                case Key.D1:
                case Key.D2:
                case Key.D3:
                case Key.D4:
                case Key.D5:
                case Key.D6:
                case Key.D7:
                case Key.D8:
                case Key.D9:
                case Key.NumPad0:
                case Key.NumPad1:
                case Key.NumPad2:
                case Key.NumPad3:
                case Key.NumPad4:
                case Key.NumPad5:
                case Key.NumPad6:
                case Key.NumPad7:
                case Key.NumPad8:
                case Key.NumPad9:
                    break;

                case Key.Back:
                    if (string.IsNullOrWhiteSpace(textBox.Text) || textBox.CaretIndex == 0)
                    {
                        if (textBox != PinDigit1)
                        {
                            textBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                            e.Handled = true;
                        }
                    }
                    break;

                case Key.Left:
                    if (textBox.CaretIndex == 0 && textBox != PinDigit1)
                    {
                        textBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                        e.Handled = true;
                    }
                    break;

                case Key.Right:
                    if (textBox.CaretIndex == textBox.Text.Length && textBox != PinDigit4)
                    {
                        textBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                        e.Handled = true;
                    }
                    break;

                case Key.Tab:
                case Key.Enter:
                    break;

                case Key.C:
                    if (e.KeyboardDevice.Modifiers != ModifierKeys.Control)
                    {
                        e.Handled = true;
                    }
                    break;

                case Key.V:
                    if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                    {
                        string clipboardText = Clipboard.GetText();
                        if (clipboardText.Length == 4 && clipboardText.All(char.IsDigit))
                        {
                            this.Text = clipboardText;
                        }
                        e.Handled = true;
                    }
                    else
                    {
                        e.Handled = true;
                    }
                    break;

                case Key.Delete:
                    break;

                default:
                    e.Handled = true;
                    break;
            }
        }
    }
}