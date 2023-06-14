using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Demo.Panels
{
    /// <summary>
    /// Interaction logic for DemoSidePanel.xaml
    /// </summary>
    public partial class SidePanelView : UserControl
    {
        public SidePanelView()
        {
            InitializeComponent();
        }

        private void AddToCollection(object sender, RoutedEventArgs e)
        {
            DemoViewModel.Instance.Collection.Add("s");
        }
    }
}