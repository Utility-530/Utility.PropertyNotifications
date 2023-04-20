using System.Windows;
using Utility.Collections;

namespace Utility.WPF.Demo.Collections
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Collection.Context = System.Threading.SynchronizationContext.Current;
            JulMar.Windows.Collections.Collection.Context = System.Threading.SynchronizationContext.Current;

        }
    }
}
