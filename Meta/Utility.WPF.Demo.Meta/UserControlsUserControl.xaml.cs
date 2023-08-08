using System.Windows.Controls;
using Utility.WPF.Meta;

namespace Utility.WPF.Demo.Meta
{
    /// <summary>
    /// Interaction logic for UserControlsUserControl.xaml
    /// </summary>
    public partial class UserControlsUserControl : UserControl
    {
        public UserControlsUserControl()
        {
            InitializeComponent();
            this.Content = new UserControlsGrid();
        }
    }
}
