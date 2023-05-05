using System.Windows;
using GraphShape.Controls;
using System.Windows.Controls;
using DryIoc;

namespace Utility.GraphShapes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class GraphUserControl : UserControl
    {

        public GraphUserControl(IContainer container)
        {
            InitializeComponent();
            this.DockPanel.DataContext = container.Resolve<GraphController>();
        }
   
        private void OnRelayoutClick(object sender, RoutedEventArgs args)
        {
            Layout.Relayout();
        }

        private void OnSelectedVertexChangeClick(object sender, RoutedEventArgs e)
        {

        }
    }

    public class PocGraphLayout : GraphLayout<PocVertex, PocEdge, PocGraph>
    {
    }
}
