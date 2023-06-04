using DryIoc;
using Utility.Infrastructure;
using Utility.PropertyTrees.Services;
using Utility.PropertyTrees.WPF.Demo.Infrastructure;

namespace Utility.PropertyTrees.WPF.Demo.Views
{
    /// <summary>
    /// Interaction logic for CommandView.xaml
    /// </summary>
    public partial class CommandView : UserControl
    {
        private ModelViewModel viewModel => container.Resolve<ModelViewModel>();
        private IModelController controller => container.Resolve<IModelController>();

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
            BaseObject.Resolver.Clear();
            //var graphWindow = new Window { Content = new GraphUserControl(container) };
            //ScreenHelper.SetOnFirstScreen(graphWindow);
            //graphWindow.Show();
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
            controller.OnNext(new RefreshRequest());
        }
    }
}
