using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.Trees.Abstractions;
using Utility.WPF.Factorys;

namespace Utility.WPF.Controls.ComboBoxes
{

    public class TreeSelectorBehavior : Behavior<ComboBoxTreeView>
    {
        private HierarchicalDataTemplate hierarchicalDataTemplate;

        public TreeSelectorBehavior()
        {

        }


        protected override void OnAttached()
        {
            hierarchicalDataTemplate = TemplateGenerator.CreateHierarcialDataTemplate(() =>
            {
                var contentControl = new ContentControl();
                contentControl.SetBinding(ContentControl.ContentProperty, new Binding() { Path = new PropertyPath(nameof(ITree.Data)) });
                return contentControl;
            });
            AssociatedObject.ItemTemplate = hierarchicalDataTemplate;
            hierarchicalDataTemplate.ItemsSource = new Binding(nameof(ITree.Items));
            AssociatedObject.ParentPath = nameof(IReadOnlyTree.Parent);

            base.OnAttached();
        }
    }
}
