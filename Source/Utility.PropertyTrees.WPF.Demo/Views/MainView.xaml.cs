using DryIoc;
using ModernWpf.Controls;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Utility.Graph.Shapes;
using Utility.PropertyTrees.WPF.Demo.Infrastructure;
using Utility.PropertyTrees.WPF.Demo.Views;
using static Utility.PropertyTrees.WPF.Demo.LightBootStrapper;

namespace Utility.PropertyTrees.WPF.Demo
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
            Focus();
            //ViewModelTree.Engine = container.Resolve<ViewModelEngine>();
            //PropertyTree.Engine = new Engine(masterNode);
        }


    
    }
}