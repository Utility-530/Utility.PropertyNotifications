using System.Linq;
using System.Windows;

namespace Utility.WPF.Demo.FileSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (var dic in Application.Current.Resources.MergedDictionaries.ToArray()
                .Where(a => a.Source.ToString().Contains("Material") &&
                            a.Source.ToString().Contains("MaterialDesignExtensions") == false))
            {
                Application.Current.Resources.MergedDictionaries.Remove(dic);
            }
        }
    }
}