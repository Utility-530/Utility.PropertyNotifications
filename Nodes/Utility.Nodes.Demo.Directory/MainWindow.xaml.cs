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
using Utility.WPF.Controls.Trees;
using Utility.WPF;
using System.Collections;
using Splat;
using Utility.Models.Trees;
using Utility.Trees.Abstractions;
using System.Globalization;

namespace Utility.Nodes.Demo.Directory
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var treeView = new TreeView();
            var parent = new ComboTreeViewItem()
            {
                Width = 200,
                Height = 30,
                //Edit = new DataFileModel { Name = "new_" },
                ItemTemplate = this.Resources["ItemTemplate"] as DataTemplate,
                Style= this.Resources["ComboExStyle"] as Style, 
                ItemContainerStyle= this.Resources["DefaultStyle"] as Style, 
                EditTemplate = this.Resources["EditTemplate"] as DataTemplate,
                SelectionTemplate = this.Resources["SelectionTemplate"] as DataTemplate,
                TreeView = treeView
                //Selection = new Binding("SelectionItem.Data") { RelativeSource= RelativeSource.Self, Converter= new DataConverter()  }
            };

            parent.SetBinding(ItemsControl.DataContextProperty, new Binding("Selection") { Source = Locator.Current.GetService<MainViewModel>(), Mode=BindingMode.OneWay });

            parent.FinishEdit += Parent_Add;


            treeView.SelectedItemChanged += (s, e) =>
            {

            };
            treeView.Items.Add(parent);
            this.Content = treeView;
        }

        private void Parent_Add(object sender, EditRoutedEventArgs e)
        {
            if (e is EditRoutedEventArgs { IsAccepted: true, Edit: { } instance } value)
            {
                value.Handled = true;

                if (Locator.Current.GetService<MainViewModel>().Selection is ITree {  } tree)
                {
                    tree.Add(tree.ToTree(instance).Result);
                }
            }
        }

        private static void Add(CustomTreeViewItem parent, TreeViewItem child)
        {
            child.Selected += (s, e) => { if (parent is ISelection selection) selection.Select(child.Header); };// selection.Selection = child.Header; };
            parent.Items.Add(child);
        }

        TreeViewItem Make(string header)
        {
            return new TreeViewItem
            {
                Header = header,
                HeaderTemplate = this.Resources["ItemTemplate"] as DataTemplate,
                Style = this.Resources["GenericTreeViewItem"] as Style
            };

        }
    }

    internal class DataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}