using System.Windows;
using System.Windows.Controls;
using Utility.WPF.Controls.Objects;

namespace Utility.WPF.Demo.Objects
{
    /// <summary>
    /// Interaction logic for JsonViewUserControl.xaml
    /// </summary>
    public partial class JsonObjectUserControl : UserControl
    {
        public JsonObjectUserControl()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Clipboard.ContainsText(TextDataFormat.Text))
            {
                string clipboardText = Clipboard.GetText(TextDataFormat.Text);
                JsonControl.Json = clipboardText;
            }
        }
    }
}