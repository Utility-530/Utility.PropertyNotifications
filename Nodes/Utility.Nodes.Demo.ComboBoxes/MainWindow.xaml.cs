using System.Reflection;
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
using Utility.Interfaces.NonGeneric;
using Utility.Trees;
using Utility.WPF.Controls.ComboBoxes;
using Utility.Meta;

namespace Utility.Nodes.Demo.ComboBoxes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DirectorySelector.ItemsSource = new DirectoryNode(new System.IO.DirectoryInfo(@"C:\")).Items;

            Type2Selector.Assemblies = new Assembly[] { typeof(Tree).Assembly, new SystemAssembly() };
            Type2Selector.Type = typeof(string);
        }
    }
}