using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Utility.WPF.Demo.Controls
{
    public partial class PinBoxUserControl
    {
        public PinBoxUserControl()
        {
            InitializeComponent();

            // Subscribe to TextChanged event
            PinBoxControl.TextChanged += PinBoxControl_TextChanged;
        }

        private void PinBoxControl_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Update the result display
            if (string.IsNullOrEmpty(PinBoxControl.Text))
            {
                ResultText.Text = "----";
                ResultText.Foreground = new SolidColorBrush(Color.FromRgb(204, 204, 204));
                VerifyButton.IsEnabled = false;
            }
            else
            {
                ResultText.Text = PinBoxControl.Text;
                ResultText.Foreground = new SolidColorBrush(Color.FromRgb(102, 126, 234));

                // Enable verify button only when PIN is complete (4 digits)
                VerifyButton.IsEnabled = PinBoxControl.Text.Length == 4;
            }

            // Hide message when user starts typing again
            MessageBorder.Visibility = Visibility.Collapsed;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear the PIN
            PinBoxControl.Text = string.Empty;

            // Focus back to the PIN box
            PinBoxControl.Focus();

            // Hide any messages
            MessageBorder.Visibility = Visibility.Collapsed;
        }

        private void VerifyButton_Click(object sender, RoutedEventArgs e)
        {
            string enteredPin = PinBoxControl.Text;

            // Example verification (in real app, verify against stored PIN)
            string correctPin = "1234"; // Demo purposes only

            if (enteredPin == correctPin)
            {
                // Show success message
                MessageBorder.Background = new SolidColorBrush(Color.FromRgb(212, 237, 218));
                MessageText.Foreground = new SolidColorBrush(Color.FromRgb(21, 87, 36));
                MessageText.Text = "✓ PIN verified successfully!";
                MessageBorder.Visibility = Visibility.Visible;
            }
            else
            {
                // Show error message
                MessageBorder.Background = new SolidColorBrush(Color.FromRgb(248, 215, 218));
                MessageText.Foreground = new SolidColorBrush(Color.FromRgb(114, 28, 36));
                MessageText.Text = "✗ Incorrect PIN. Please try again.";
                MessageBorder.Visibility = Visibility.Visible;

                // Optional: Clear the PIN after incorrect attempt
                // PinBoxControl.Text = string.Empty;
                // PinBoxControl.Focus();
            }
        }
    }
}