using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls.Primitives;
using Microsoft.Xaml.Behaviors;
using Utility.WPF.Reactives;

namespace Utility.WPF.Behaviors
{
    public class RemoveItemsOnSelected : Behavior<Selector>
    {
        protected override void OnAttached()
        {
            RemoveItemsOnSelectedAddItemsOnDeselected(AssociatedObject);
            base.OnAttached();
        }

        private static void RemoveItemsOnSelectedAddItemsOnDeselected(Selector selector)
        {
            //var collection = new ObservableCollection<object>(selector.ItemsSource.Cast<object>());
            Stack<(int, object)> removedObjects = new();
            IEnumerable itemsSource = null;
            selector.Changes().Subscribe(a =>
            {
                itemsSource = selector.ItemsSource;
                var itemsSourceCollection = selector.ItemsSource.Cast<object>().ToList();
                var index = selector.Items.IndexOf(a);
                for (int i = selector.Items.Count - 1; i > -1; i--)
                {
                    if (index != i)
                    {
                        removedObjects.Push((i, itemsSourceCollection[i]));
                        itemsSourceCollection.RemoveAt(i);
                    }
                }
                selector.ItemsSource = itemsSourceCollection;
            });

            selector.Removals().Where(rem => rem.Cast<object>().Any()).Subscribe(a =>
            {
                selector.ItemsSource = itemsSource;
            });
        }
    }
}