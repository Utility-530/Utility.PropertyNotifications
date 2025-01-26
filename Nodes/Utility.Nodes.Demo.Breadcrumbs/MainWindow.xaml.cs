using System.Windows;
using Utility.Nodes.Filters;

namespace Utility.Nodes.Breadcrumbs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


            NodeSource.Instance
                .Single(nameof(Factory.BreadcrumbRoot))
                .Subscribe(node =>
                {
                    this.DataContext = node;
                });
        }
    }


}