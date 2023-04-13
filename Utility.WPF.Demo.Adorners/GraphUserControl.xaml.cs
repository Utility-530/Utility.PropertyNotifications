using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Utility.WPF.Adorners;
using Utility.WPF.Adorners.Infrastructure;
using Utility.WPF.Helpers;

namespace Utility.WPF.Demo.Adorners
{
    /// <summary>
    /// Interaction logic for GraphUserControl.xaml
    /// </summary>
    public partial class GraphUserControl : UserControl
    {
        VerticalAxisAdorner adorner;
        public GraphUserControl()
        {
            InitializeComponent();
            adorner = new VerticalAxisAdorner(Grid);
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(Grid);
            layer.AddIfMissingAdorner(adorner);
        }
        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            foreach (UIElement ui in Grid.Children)
            {
                ui.RemoveAdorners();
            }

            Grid.RemoveAdorners();
        }
    }
}
