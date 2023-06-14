using DynamicData;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Utility.WPF.Reactive;

namespace Utility.WPF.Attached
{
    public class SelectorEx : Selector
    {
        #region DoubleClick

        public static readonly DependencyProperty DoubleClickItemCommandProperty =
            DependencyProperty.RegisterAttached("DoubleClickItemCommand", typeof(ICommand), typeof(SelectorEx), new PropertyMetadata(null, OnDoubleClickItemCommand));

        public static ICommand GetDoubleClickItemCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(DoubleClickItemCommandProperty);
        }

        public static void SetDoubleClickItemCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(DoubleClickItemCommandProperty, value);
        }

        private static void OnDoubleClickItemCommand(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Selector selector && e.NewValue is ICommand command)
            {
                // Remove the handler if it exist to avoid memory leaks
                selector.MouseDoubleClick -= UIElement_MouseDoubleClick;

                // the property is attached so we attach the Drop event handler
                selector.MouseDoubleClick += UIElement_MouseDoubleClick;
            }
        }

        private static void UIElement_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Sanity check just in case this was somehow send by something else
            // Is there a selected item that was Single clicked?
            // There may not be a command bound to this after all
            if (!(sender is Selector selector) || selector.SelectedIndex == -1 || !(GetDoubleClickItemCommand(selector) is ICommand doubleclickCommand))
                return;

            // Check whether this attached behaviour is bound to a RoutedCommand
            if (doubleclickCommand is RoutedCommand doubleclickRoutedCommand)
                // Execute the routed command
                doubleclickRoutedCommand.Execute(selector.SelectedItem, selector);
            else
                // Execute the Command as bound delegate
                doubleclickCommand.Execute(selector);
        }

        #endregion DoubleClick

        #region SingleClick

        public static readonly DependencyProperty SingleClickItemCommandProperty =
       DependencyProperty.RegisterAttached("SingleClickItemCommand", typeof(ICommand), typeof(SelectorEx), new PropertyMetadata(null, OnSingleClickItemCommand));

        public static ICommand GetSingleClickItemCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(SingleClickItemCommandProperty);
        }

        public static void SetSingleClickItemCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(SingleClickItemCommandProperty, value);
        }

        private static void OnSingleClickItemCommand(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Remove the handler if it exist to avoid memory leaks
            if (d is Selector selector)
            {
                selector.MouseLeftButtonUp -= UIElement_MouseSingleClick;

                // the property is attached so we attach the Drop event handler
                selector.MouseLeftButtonUp += UIElement_MouseSingleClick;
            }
        }

        private static void UIElement_MouseSingleClick(object sender, MouseButtonEventArgs e)
        {
            // Sanity check just in case this was somehow send by something else
            // Is there a selected item that was Single clicked?
            // There may not be a command bound to this after all
            if (!(sender is Selector selector) || selector.SelectedIndex == -1 || !(GetSingleClickItemCommand(selector) is ICommand singleclickCommand))
                return;

            // Check whether this attached behaviour is bound to a RoutedCommand
            if (singleclickCommand is RoutedCommand)
                // Execute the routed command
                (singleclickCommand as RoutedCommand)?.Execute(selector.SelectedItem, selector);
            else
                // Execute the Command as bound delegate
                singleclickCommand.Execute(selector);
        }

        #endregion SingleClick

        #region SelectedItem

        public static readonly DependencyProperty SelectedItemTemplateProperty =
       DependencyProperty.RegisterAttached("SelectedItemTemplate", typeof(DataTemplate), typeof(SelectorEx), new PropertyMetadata(null, OnSelectedItemTemplate));

        public static DataTemplate GetSelectedItemTemplate(DependencyObject obj)
        {
            return (DataTemplate)obj.GetValue(SelectedItemTemplateProperty);
        }

        public static void SetSelectedItemTemplate(DependencyObject obj, DataTemplate value)
        {
            obj.SetValue(SelectedItemTemplateProperty, value);
        }

        private static void OnSelectedItemTemplate(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Selector selector)
            {
                DataTemplate originalTemplate = selector.ItemTemplate;
                _ = selector
                    .SelectSelectionAddChanges()
                    .Subscribe(add =>
                    {
                        foreach (ListBoxItem lbx in selector.Items.Cast<object>().Select(a => selector.ItemContainerGenerator.ContainerFromItem(a)).OfType<ListBoxItem>())
                        {
                            if (lbx.IsSelected)
                            {
                                originalTemplate = selector.ItemTemplate;
                                lbx.ContentTemplate = GetSelectedItemTemplate(d);
                            }
                            else
                                lbx.ContentTemplate = originalTemplate ?? selector.ItemTemplate;
                        }
                    });
            }
        }

        #endregion SelectedItem

        //https://stackoverflow.com/questions/4672867/can-i-use-a-different-template-for-the-selected-item-in-a-wpf-combobox-than-for
        public class ComboBoxTemplateSelector : DataTemplateSelector
        {
            public DataTemplate? SelectedItemTemplate { get; set; }
            public DataTemplateSelector? SelectedItemTemplateSelector { get; set; }
            public DataTemplate? DropdownItemsTemplate { get; set; }
            public DataTemplateSelector? DropdownItemsTemplateSelector { get; set; }

            public override DataTemplate? SelectTemplate(object item, DependencyObject container)
            {
                var itemToCheck = container;

                // Search up the visual tree, stopping at either a ComboBox or
                // a ComboBoxItem (or null). This will determine which template to use
                while (itemToCheck != null && !(itemToCheck is ComboBoxItem) && !(itemToCheck is ComboBox))
                    itemToCheck = VisualTreeHelper.GetParent(itemToCheck);

                // If you stopped at a ComboBoxItem, you're in the dropdown
                var inDropDown = itemToCheck is ComboBoxItem;

                return inDropDown
                    ? DropdownItemsTemplate ?? DropdownItemsTemplateSelector?.SelectTemplate(item, container)
                    : SelectedItemTemplate ?? SelectedItemTemplateSelector?.SelectTemplate(item, container);
            }
        }
    }
}