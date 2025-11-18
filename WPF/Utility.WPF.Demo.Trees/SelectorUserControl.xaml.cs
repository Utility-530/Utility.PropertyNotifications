using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Utility.Commands;
using Utility.Interfaces.NonGeneric;
using Utility.Models.Trees;
using Utility.Trees.Abstractions;
using NodeVM = Utility.Nodes.NodeViewModel;

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

    internal class SelectorMainViewModel
    {
        private NodeVM node = null;

        public NodeVM[] Nodes
        {
            get
            {
                return [node ??= create().Result];

                static async Task<NodeVM> create()
                {
                    NodeVM root = new StringRootModel { Name = "Big Daddy Root", Value = "a" };

                    int branches = 0;
                    int subBranches = 0;

                    for (int i = 0; i < 2; ++i)
                    {
                        ITree _child =new StringModel { Name = "Branch " + ++branches, Value = "as" };
                        root.Add(_child);
                        for (int j = 0; j < 3; ++j)
                        {
                            ITree gchild = new StringModel { Name = "Sub-Branch " + ++subBranches, Value = "aee" };
                            _child.Add(gchild);
                            for (int k = 0; k < 2; ++k)
                                gchild.Add(new StringModel { Name = "Leaf", Value = "ewa" });
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
            if (item is IRoot )
            {
                return RootStyle;
            }
            return DefaultStyle;
        }

        public Style DefaultStyle { get; set; }
        public Style RootStyle { get; set; }
    }
}