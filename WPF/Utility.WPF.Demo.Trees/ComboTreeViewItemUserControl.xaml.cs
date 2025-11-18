using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.WPF.Controls.Trees;
using Utility.WPF.Demo.Data.Model;

namespace Utility.WPF.Demo.Trees
{
    /// <summary>
    /// Interaction logic for ComboTreeViewItemUserControl.xaml
    /// </summary>
    public partial class ComboTreeViewItemUserControl : UserControl
    {
        public ComboTreeViewItemUserControl()
        {
            InitializeComponent();
            var treeView = new TreeView();
            var parent = new ComboTreeViewItem()
            {
                Width = 200,
                Height = 30,
                ItemTemplate = this.Resources[new DataTemplateKey(typeof(Character))] as DataTemplate,
                //Style= this.Resources["Demo"] as Style,
                EditTemplate = this.Resources["EditTemplate"] as DataTemplate,
                ItemContainerStyle = this.Resources["CustomTreeViewItem"] as Style,
                TreeView = treeView,
                ItemsSource = this.Resources["Characters"] as IEnumerable,
                SelectedItem = new Binding { Path = new PropertyPath("Data") }
            };
            parent.FinishEdit += Parent_Add;

            treeView.AddHandler(CustomTreeViewItem.ChangeEvent, new RoutedEventHandler(change));
            treeView.Items.Add(parent);
            this.Content = treeView;
        }

        private void change(object sender, RoutedEventArgs e)
        {
        }

        private void Parent_Add(object sender, EditRoutedEventArgs e)
        {
            MessageBox.Show($"{e.IsAccepted}");
        }

        private static void Add(CustomTreeViewItem parent, TreeViewItem child)
        {
            child.Selected += (s, e) => { if (parent is ISelection selection) selection.Select(child.Header); };// selection.Selection = child.Header; };
            parent.Items.Add(child);
        }

        private TreeViewItem Make(string header)
        {
            return new TreeViewItem
            {
                Header = header,
                HeaderTemplate = this.Resources["ItemTemplate"] as DataTemplate,
                Style = this.Resources["GenericTreeViewItem"] as Style
            };
        }
    }
}