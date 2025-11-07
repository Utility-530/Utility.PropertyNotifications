using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Demo.Controls
{
    /// <summary>
    /// Interaction logic for SplashscreenUSerControl.xaml
    /// </summary>
    public partial class SplashscreenUserControl : UserControl
    {
        public SplashscreenUserControl()
        {
            InitializeComponent();
        }

        public void Finished()
        {
            MessageBox.Show("Finished");
        }
    }
}