using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Controls.Trees
{
    public class MyTreeView : TreeView
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MyTreeViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is MyTreeViewItem;
        }
        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            if (newValue is INotifyCollectionChanged collectionChanged)
                collectionChanged.CollectionChanged += CollectionChanged_CollectionChanged;
        }

        private void CollectionChanged_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            HasItems2 = ItemsSource.Cast<object>().Any();
        }

        public bool HasItems2
        {
            get { return (bool)GetValue(HasItems2Property); }
            set { SetValue(HasItems2Property, value); }
        }

        public static readonly DependencyProperty HasItems2Property =
            DependencyProperty.Register("HasItems2", typeof(bool), typeof(MyTreeView), new PropertyMetadata(false));
    }

    public class MyTreeViewItem : TreeViewItem
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MyTreeViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is MyTreeViewItem;
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            if (newValue is INotifyCollectionChanged collectionChanged)
                collectionChanged.CollectionChanged += CollectionChanged_CollectionChanged;
        }

        private void CollectionChanged_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            HasItems2 = ItemsSource.Cast<object>().Any();
        }

        public bool HasItems2
        {
            get { return (bool)GetValue(HasItems2Property); }
            set { SetValue(HasItems2Property, value); }
        }

        public static readonly DependencyProperty HasItems2Property =
            DependencyProperty.Register("HasItems2", typeof(bool), typeof(MyTreeViewItem), new PropertyMetadata(false));


    }
}
