using System;
using System.Collections.ObjectModel;
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
            return TreeExtensions.ExploreTree(treeView, t => t.Items, (itemcollection, viewModel, treeView) =>
            {
                var treeViewItem = factory.Make(viewModel, treeView);

                treeViewItem.ItemsSource = new ObservableCollection<ItemsControl>();
                treeViewItem.ItemContainerStyleSelector = styleSelector;

                if (treeViewItem is HeaderedItemsControl headeredItemsControl)
                {
                    var binding = new Binding()
                    { 
                        Source = headeredItemsControl.Header,
                        Converter = ItemsPanelConverter,
                        Mode = BindingMode.OneTime
                    };

                    headeredItemsControl.SetBinding(ItemsControl.ItemsPanelProperty, binding);
                    headeredItemsControl.HeaderTemplateSelector = dataTemplateSelector;
                }
                (treeView.ItemsSource as ObservableCollection<ItemsControl>).Add(treeViewItem);
                //itemcollection.Add(treeViewItem);
                itemcollection.CurrentChanged += Itemcollection_CurrentChanged;
                return treeViewItem;
            },
            (itemCollection, node) => itemCollection.SourceCollection.RemoveAt(itemCollection.IndexOf(a=> (a as TreeViewItem)?.Header, node)),
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
