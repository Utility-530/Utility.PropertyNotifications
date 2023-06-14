using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows;

namespace BreadcrumbLib.Infrastructure
{

    internal class CustomItemsControl<T> : ItemsControl where T : DependencyObject, new()
    {
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
            "SelectedItem", typeof(BreadcrumbItem), typeof(CustomItemsControl<T>), new UIPropertyMetadata(null));

        public CustomItemsControl()
        {
            ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
        }

        public T SelectedItem
        {
            get => (T)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new T();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is T;
        }

        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                if (!Items.IsEmpty)
                {
                    object item = Items[Items.Count - 1];
                    DependencyObject container = ItemContainerGenerator.ContainerFromItem(item);
                    SelectedItem = (T)container;
                }
            }
        }
    }
}
