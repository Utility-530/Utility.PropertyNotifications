using System;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.Nodes;
using Utility.Trees.Abstractions;
using UtilityReactive;

namespace Views.Trees
{
    public class TreeViewBuilder : ITreeViewBuilder
    {
        public IDisposable Build(TreeView treeView, object rootViewModel, ITreeViewItemFactory factory, IValueConverter ItemsPanelConverter, DataTemplateSelector dataTemplateSelector, ITreeViewFilter filter)
        {
            return TreeExtensions.ExploreTree(treeView.Items, (itemcollection, viewModel) =>
            {

                var treeViewItem = factory.Make();
                treeViewItem.Header = viewModel;
                treeViewItem.DataContext = viewModel;

                var binding = new Binding()
                {
                    //Source = new PropertyPath()
                    Converter = ItemsPanelConverter,
                    Mode = BindingMode.OneTime
                };

                treeViewItem.SetBinding(ItemsControl.ItemsPanelProperty, binding);
                treeViewItem.HeaderTemplateSelector = dataTemplateSelector;

                itemcollection.Add(treeViewItem);
                return treeViewItem.Items;

                return itemcollection;
            },
            (a, b) => a.Remove(new TreeViewItem { Header = b }),
            rootViewModel as Node, 
            filter.Filter);
        }

        public static TreeViewBuilder Instance { get; } = new();
    }
}
