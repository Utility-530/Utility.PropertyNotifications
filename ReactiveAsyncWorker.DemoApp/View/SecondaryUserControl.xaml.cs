using System.Windows.Controls;

namespace DemoApp
{
    /// <summary>
    /// Interaction logic for SecondaryUserControl.xaml
    /// </summary>
    public partial class SecondaryUserControl : UserControl
    {
        public SecondaryUserControl()
        {
            InitializeComponent();
            this.DataContext = new SecondaryViewModel();
        }
    }
}
