using Splat;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Utility.Pipes;
using Utility.Trees.Demo.Filters;
using Utility.Trees.Demo.Filters.Infrastructure;
using Utility.Trees.WPF;

namespace Utility.Trees.Demo.Filters
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
        }

        void UpdateView(object data)
        {
            //treeViewer = DataTreeViewer(data);
            //scrollviewer.Content = treeViewer;
            //MainView.Instance.pipe_view.Content = Locator.Current.GetService<PipeController>();
            //MainView.Instance.queue_view.Content = Pipe.Instance;



            //filtertree.Content = treeViewer;
            //filtertree.ContentTemplate = this.Resources["TVF"] as DataTemplate;
            //datatemplatetree.Content = treeViewer;
            //datatemplatetree.ContentTemplate = this.Resources["DTS"] as DataTemplate;
            //styletree.Content = treeViewer;
            //styletree.ContentTemplate = this.Resources["SS"] as DataTemplate;




            //pipe_repository_tree.Content = Locator.Current.GetService<PipeRepository>();
            //pipe_repository_tree.ContentTemplate = this.Resources["Pipe_Repository_Template"] as DataTemplate;
            //repo_view.Content = Locator.Current.GetService<PipeRepository>();


        }
    }
}