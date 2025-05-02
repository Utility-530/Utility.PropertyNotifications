using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Linq;
using System.Windows.Controls.Primitives;

namespace Leepfrog.WpfFramework.Behaviors
{
    /// <summary>
    /// Behaviour to be applied to Listbox type controls
    /// When listbox selection changes, it will be scrolledintoview
    /// </summary>
    public static class ScrollSelectedItemIntoViewBehavior
    {
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(ScrollSelectedItemIntoViewBehavior),
            new UIPropertyMetadata(false, OnIsEnabledChanged));

        public static bool GetIsEnabled(DependencyObject d)
        {
            return (bool)d.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(DependencyObject d, bool value)
        {
            d.SetValue(IsEnabledProperty, value);
        }
        private static void OnIsEnabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var selector = sender as Selector;
            // IF SENDER IS NOT A SELECTOR, JUST EXIT
            if (selector == null)
            {
                return;
            }

            bool isEnabled = (bool)(e.NewValue);

            // GET DESCRIPTOR OF LABEL'S TARGET PROPERTY
            if (isEnabled)
            {
                // REGISTER FOR UPDATES WHENEVER THE TARGET PROPERTY CHANGES...
                selector.AddHandler(Selector.SelectionChangedEvent, new RoutedEventHandler(selectionChangedHandler));
            }
            else
            {
                // UNREGISTER
                selector.RemoveHandler(Selector.SelectionChangedEvent, new RoutedEventHandler(selectionChangedHandler));
            }
        }

        private static void selectionChangedHandler(object sender, RoutedEventArgs e)
        {
            if (sender is ListBox)
            {
                var listbox = (sender as ListBox);
                if (listbox.SelectedItem != null)
                {
                    listbox.Dispatcher.BeginInvoke(
                        new Action(() =>
                        {
                            listbox.UpdateLayout();
                            if (listbox.SelectedItem != null)
                            {
                                listbox.ScrollIntoView(listbox.SelectedItem);
                            }
                        }
                        ));
                }
            }
        }
    }
}
