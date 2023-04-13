using System.Windows;
using System.Windows.Controls;
using Utility.WPF.Controls.FileSystem.Infrastructure;

namespace Utility.WPF.Demo.FileSystem
{
    /// <summary>
    /// Interaction logic for FolderBrowserView.xaml
    /// </summary>
    public partial class FolderBrowserView : UserControl
    {
        public FolderBrowserView()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void FolderBrowser1_OnTextChange(object sender, TextChangedRoutedEventArgs e)
        {
        }
    }
}