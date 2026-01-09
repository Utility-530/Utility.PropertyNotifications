using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Xaml.Behaviors;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Trees.Abstractions;
using Utility.WPF.Factorys;
using Utility.WPF.Helpers;

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
            AssociatedObject.ItemTemplateSelector = DataTemplateSelector();
            AssociatedObject.SelectedItemTemplateSelector = SelectedTemplateSelector();
            AssociatedObject.ParentPath = nameof(IGetParent<>.Parent);
            base.OnAttached();
        }

        public virtual DataTemplateSelector DataTemplateSelector()
        {
            DataTemplate hierarchicalDataTemplate = null;
            return DataTemplateHelper.CreateSelector((a, b) =>
            {
                return hierarchicalDataTemplate ??= TemplateGenerator.CreateHierarcialDataTemplate(() =>
                {
                    var contentControl = new ContentControl();
                    contentControl.SetBinding(ContentControl.ContentProperty, new Binding() { Path = new PropertyPath(nameof(IGetData.Data)) });
                    return contentControl;
                }, new Binding { Mode = BindingMode.OneWay, Path = new PropertyPath(nameof(IReadOnlyTree.Children)) });
            });
        }

        public virtual DataTemplateSelector SelectedTemplateSelector() => DataTemplateSelector();
    }
}