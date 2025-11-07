using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Xaml.Behaviors;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
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
                contentControl.SetBinding(ContentControl.ContentProperty, new Binding() { Path = new PropertyPath(nameof(IGetValue.Value)) });
                return contentControl;
            }, new Binding { Mode = BindingMode.OneWay, Path = new PropertyPath(nameof(IReadOnlyTree.Children)) });
            AssociatedObject.ItemTemplate = hierarchicalDataTemplate;
            AssociatedObject.ParentPath = nameof(IGetParent<>.Parent);

            base.OnAttached();
        }
    }
}