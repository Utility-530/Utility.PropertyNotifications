using NetFabric.Hyperlinq;
using System;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.Helpers.NonGeneric;
using Utility.Nodes;
using Utility.WPF.Nodes;

namespace Views.Trees
{
    public class TreeViewBuilder : ITreeViewBuilder
    {
        public IDisposable Build(TreeView treeView, object rootViewModel, ITreeViewItemFactory factory, IValueConverter ItemsPanelConverter, StyleSelector styleSelector, DataTemplateSelector dataTemplateSelector, ITreeViewFilter filter)
        {
            return TreeExtensions.ExploreTree(treeView.Items, (itemcollection, viewModel) =>
            {

                var treeViewItem = factory.Make();
                treeViewItem.Header = viewModel;
                treeViewItem.DataContext = viewModel;
                treeViewItem.ItemContainerStyleSelector = styleSelector;
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
            },
            (itemCollection, node) => itemCollection.RemoveAt(itemCollection.IndexOf(a=> (a as TreeViewItem)?.Header, node)),
            a => a.Clear(),
            rootViewModel as Node,
            filter.Filter);
        }

        public static TreeViewBuilder Instance { get; } = new();
    }
}
