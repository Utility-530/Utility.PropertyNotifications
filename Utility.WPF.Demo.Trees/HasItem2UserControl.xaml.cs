using System.Windows.Controls;
using Utility.Trees;

namespace Utility.WPF.Demo.Trees
{
    /// <summary>
    /// Interaction logic for HasItem2UserControl.xaml
    /// </summary>
    public partial class HasItem2UserControl : UserControl
    {
        public HasItem2UserControl()
        {
            InitializeComponent();

            TreeView.ItemsSource = new[] { tree }; 
            MyTreeView.ItemsSource = new[] { tree }; 
        }

        Tree tree = new Tree("root")
        {

        };


        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            tree.Add("new");
        }
    }


    
}
