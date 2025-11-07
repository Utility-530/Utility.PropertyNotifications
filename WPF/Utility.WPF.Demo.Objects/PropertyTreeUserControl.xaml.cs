using System.Windows.Controls;

namespace Utility.WPF.Demo.Objects
{
    /// <summary>
    /// Interaction logic for PropertyTreeUserControl.xaml
    /// </summary>
    //[ViewAttribute(0)]
    public partial class PropertyTreeUserControl : UserControl
    {
        public PropertyTreeUserControl()
        {
            SQLitePCL.Batteries.Init();
            InitializeComponent();
        }
    }
}