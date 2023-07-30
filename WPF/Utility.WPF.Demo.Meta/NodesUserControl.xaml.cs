using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Utility.WPF.Meta;

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
