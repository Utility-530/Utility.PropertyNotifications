using System.Collections;
using System.Windows;
using System.Windows.Controls;
using Utility.WPF.Controls.Trees;

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
            var parent = new ComboTreeViewItem() {
                Width = 200,
                Height = 30, 
                //Style= this.Resources["Demo"] as Style, 
                EditTemplate = this.Resources["EditTemplate"] as DataTemplate,
                ItemsSource = this.Resources["Characters"] as IEnumerable,
            };
            parent.FinishEdit += Parent_Add;
            //var treeViewItemA = Make("A");
            //var treeViewItemB = Make("B");
            //var treeViewItemC = Make("C");
            //var treeViewItemD = Make("D");
            //Add(parent, treeViewItemA);
            //Add(parent, treeViewItemB);
            //Add(parent, treeViewItemC);
            //Add(parent, treeViewItemD);
            var treeView = new TreeView();
            treeView.Items.Add(parent);
            this.Content = treeView;
        }

        private void Parent_Add(object sender, NewObjectRoutedEventArgs e)
        {
            MessageBox.Show($"{e.IsAccepted}");
        }

        private static void Add(ComboTreeViewItem parent, TreeViewItem child)
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
}
