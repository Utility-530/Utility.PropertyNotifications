using System.Threading;
using System.Windows;
using Utility.Collections;

namespace Utility.Nodes.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Collection.Context = SynchronizationContext.Current;
        }
    }
}
