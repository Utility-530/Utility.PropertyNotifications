using System.Windows.Controls;
using Utility.WPF.Nodes;

namespace Utility.WPF.Demo.Meta
{
    /// <summary>
    /// Interaction logic for NodesUserControl.xaml
    /// </summary>
    public partial class NodesUserControl : UserControl
    {
        public NodesUserControl()
        {
            InitializeComponent();
            this.Content = new ProjectRootNode();
        }
    }
}
