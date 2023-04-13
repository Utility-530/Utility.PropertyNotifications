using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Demo.Panels
{
    /// <summary>
    /// Interaction logic for DemoCornerPanel.xaml
    /// </summary>
    public partial class CornerPanelView : UserControl
    {
        public CornerPanelView()
        {
            InitializeComponent();
        }

        private void ShowButton_Checked(object sender, RoutedEventArgs e)
        {
            CornerPanelControl.ShowLine();
            CornerPanelControl2.ShowLine();
        }

        private void AddToCollection(object sender, RoutedEventArgs e)
        {
            DemoViewModel.Instance.Collection.Add("c");
        }
    }
}