using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.Interfaces.Generic;
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
            }, new Binding { Mode = BindingMode.OneWay, Path = new PropertyPath(nameof(IReadOnlyTree.Items)) });
            AssociatedObject.ItemTemplate = hierarchicalDataTemplate;
            AssociatedObject.ParentPath = nameof(IGetParent<>.Parent);

            base.OnAttached();
        }
    }
}
