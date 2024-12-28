using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.Trees.Abstractions;
using Utility.WPF.Controls.Trees;
using Utility.WPF.Factorys;

namespace Utility.WPF.Controls.ComboBoxes
{
    public class TreeSelector : ComboBoxTreeView
    {
        private HierarchicalDataTemplate hierarchicalDataTemplate;


        //public static readonly DependencyProperty ChildrenNameProperty = DependencyProperty.Register("ChildrenName", typeof(string), typeof(TreeSelector), new PropertyMetadata(Changed));

        //private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    if (d is TreeSelector typeSelector)
        //    {
        //        typeSelector.hierarchicalDataTemplate.ItemsSource = new Binding((string)e.NewValue);
        //    }
        //}

        public TreeSelector()
        {
            hierarchicalDataTemplate = TemplateGenerator.CreateHierarcialDataTemplate(() =>
            {
                var contentControl = new ContentControl();
                contentControl.SetBinding(ContentControl.ContentProperty, new Binding() { Path = new PropertyPath(nameof(ITree.Data)) });
                return contentControl;
            });
            ItemTemplate = hierarchicalDataTemplate;
            hierarchicalDataTemplate.ItemsSource = new Binding(nameof(ITree.Items));
            ParentPath = nameof(IReadOnlyTree.Parent);
        }


        //public string ChildrenName
        //{
        //    get { return (string)GetValue(ChildrenNameProperty); }
        //    set { SetValue(ChildrenNameProperty, value); }
        //}
    }
}
