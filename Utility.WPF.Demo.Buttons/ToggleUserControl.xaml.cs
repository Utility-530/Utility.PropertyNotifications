using System.ComponentModel;
using System.Windows.Controls;

namespace Utility.WPF.Demo.Buttons
{
    /// <summary>
    /// Interaction logic for ToggleUserControl.xaml
    /// </summary>
    public partial class ToggleUserControl : UserControl
    {
        private readonly TypeConverter converter;

        public ToggleUserControl()
        {
            InitializeComponent();

        }

    }
}