using System.Windows.Controls;

namespace Utility.WPF.Demo.Animation
{
    /// <summary>
    /// Interaction logic for BarUserControl.xaml
    /// </summary>
    public partial class BarUserControl : UserControl
    {
        public BarUserControl()
        {
            InitializeComponent();
            this.Loaded += BarUserControl_Loaded;
        }

        private void BarUserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            toggleButton1.IsChecked = true;

        }
    }
}