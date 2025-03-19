using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Utility.Commands;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
using Utility.Models.Trees;
using Utility.Nodes;
using Utility.Trees.Abstractions;

namespace Utility.WPF.Demo.Trees
{
    /// <summary>
    /// Interaction logic for SelectorUserControl.xaml
    /// </summary>
    public partial class SelectorUserControl : UserControl
    {
        public SelectorUserControl()
        {
            InitializeComponent();
        }

        private void tree_FinishEdit(object sender, EditRoutedEventArgs e)
        {
            MessageBox.Show((e.Edit as StringModel).Name);
        }
    }


    public class EditConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new StringModel { Name = "New" };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class SelectorMainViewModel
    {
        Node node = null;
        public Node[] Nodes
        {
            get
            {
                return [node ??= create().Result];

                static async Task<Node> create()
                {
                    Node root = new Node(new StringRootModel { Name = "Big Daddy Root", Value="a"});

                    int branches = 0;
                    int subBranches = 0;

                    for (int i = 0; i < 2; ++i)
                    {
                        ITree _child = await root.ToTree(new StringModel { Name = "Branch " + ++branches, Value = "as" });
                        root.Add(_child);
                        for (int j = 0; j < 3; ++j)
                        {
                            ITree gchild = await _child.ToTree(new StringModel { Name = "Sub-Branch " + ++subBranches, Value = "aee" });
                            _child.Add(gchild);
                            for (int k = 0; k < 2; ++k)
                                gchild.Add(await gchild.ToTree(new StringModel { Name = "Leaf" , Value = "ewa" }));
                        }
                    }

                    return root;
                }
            }
        }


        public ICommand FinishEditCommand { get; } = new Command(() =>
        {

        });

    }

    public class StringRootModel : StringModel, IRoot
    {
        public StringRootModel()
        {
        }
    }

    public class CustomStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is INode { Data: IRoot data })
            {
                return RootStyle;

            }
            return DefaultStyle;
        }

        public Style DefaultStyle { get; set; }
        public Style RootStyle { get; set; }
    }


}
