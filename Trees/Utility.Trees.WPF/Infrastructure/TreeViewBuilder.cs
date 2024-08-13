using System;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.Extensions;
using Utility.Helpers.NonGeneric;
using Utility.Interfaces.NonGeneric;
using Utility.Trees.WPF.Abstractions;

namespace Views.Trees
{
    public class TreeViewBuilder : ITreeViewBuilder
    {
        public IDisposable Build(ItemsControl treeView, IItems rootViewModel, ITreeViewItemFactory factory, IValueConverter ItemsPanelConverter, StyleSelector styleSelector, DataTemplateSelector dataTemplateSelector, ITreeViewFilter filter)
        {
            return TreeExtensions.ExploreTree(treeView, t =>t.Items, (itemcollection, viewModel, treeView) =>
            {
                var treeViewItem = factory.Make(viewModel, treeView);

                treeViewItem.ItemContainerStyleSelector = styleSelector;
                var binding = new Binding()
                {
                    //Source = new PropertyPath(),
                    Source = treeViewItem.Header,
                    Converter = ItemsPanelConverter,
                    Mode = BindingMode.OneTime
                };

                treeViewItem.SetBinding(ItemsControl.ItemsPanelProperty, binding);
                treeViewItem.HeaderTemplateSelector = dataTemplateSelector;

                itemcollection.Add(treeViewItem);
                itemcollection.CurrentChanged += Itemcollection_CurrentChanged;
                return treeViewItem;
            },
            (itemCollection, node) => itemCollection.RemoveAt(itemCollection.IndexOf(a=> (a as TreeViewItem)?.Header, node)),
            a => a.Clear(),
            rootViewModel,
            filter.Convert);
        }

        private void Itemcollection_CurrentChanged(object? sender, EventArgs e)
        {
   
        }

        public static TreeViewBuilder Instance { get; } = new();
    }
}
