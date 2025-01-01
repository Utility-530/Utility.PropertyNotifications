using System.Text;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
                .Single(nameof(Factory.Assembly))
                .Subscribe(node =>
                {
                    this.DataContext = node;
                });
        }
    }


}