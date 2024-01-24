using System.Windows.Controls;
using Utility.Enums;

namespace Utility.WPF.Demo.View
{
    /// <summary>
    /// Interaction logic for EnumUserControl.xaml
    /// </summary>
    public partial class EnumUserControl : UserControl
    {
        public EnumUserControl()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }

    public class EnumViewModel
    {
        public Emotion Emotion { get; set; } = Emotion.Confusion; 
    }
}