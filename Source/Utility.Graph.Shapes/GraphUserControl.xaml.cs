using System.Windows;
using GraphShape.Controls;
using System.Windows.Controls;
using DryIoc;

namespace Utility.Graph.Shapes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class GraphUserControl : DockPanel
    {
        private readonly IContainer container;

        GraphController graphController => container.Resolve<GraphController>();

        public GraphUserControl(IContainer container)
        {
            InitializeComponent();
            this.container = container;
            this.DataContext = graphController;
        }
   
        private void OnRelayoutClick(object sender, RoutedEventArgs args)
        {
            Layout.Relayout();
        }
    }

    public class PocGraphLayout : GraphLayout<PocVertex, PocEdge, PocGraph>
    {
    }
}
