using DryIoc;
using System.Windows;
using System.Windows.Controls;
using Utility.Collections;
using Utility.Graph.Shapes;
using Utility.PropertyTrees.WPF.Demo.Infrastructure;
using static Utility.PropertyTrees.WPF.Demo.LightBootStrapper;

namespace Utility.PropertyTrees.WPF.Demo.Views
{
    /// <summary>
    /// Interaction logic for CommandView.xaml
    /// </summary>
    public partial class CommandView : UserControl
    {
        private MainViewModel viewModel => container.Resolve<MainViewModel>();

        public CommandView()
        {
            InitializeComponent();
          
        }

        private void show_viewModels_click(object sender, RoutedEventArgs e)
        {
            var controlWindow = new Window { Content = new ViewModelView() };
            ScreenHelper.SetOnLastScreen(controlWindow);
            controlWindow.Show();
        }

        private void show_graph_click(object sender, RoutedEventArgs e)
        {
            var graphWindow = new Window { Content = new GraphUserControl(container) };
            ScreenHelper.SetOnFirstScreen(graphWindow);
            graphWindow.Show();
            //AutoObject.Resolver.Initialise();
        }

        private void show_history_click(object sender, RoutedEventArgs e)
        {
            var controlWindow = new Window { Content = container.Resolve<HistoryViewModel>() };
            ScreenHelper.SetOnLastScreen(controlWindow);
            controlWindow.Show();
        }

        private void show_templates_click(object sender, RoutedEventArgs e)
        {
            new Window { Content = new TemplatesView(container) }.Show();
        }

        private void refresh_click(object sender, RoutedEventArgs e)
        {
            viewModel.OnNext(new RefreshRequest());
        }
    }
}
