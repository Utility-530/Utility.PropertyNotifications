using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Linq;
using System.Windows.Threading;
using System.Windows.Documents;
using Leepfrog.WpfFramework.Controls;

namespace Leepfrog.WpfFramework.Behaviors
{
    /// <summary>
    /// Behaviour to be applied to any control
    /// When the isLoading flag is set, overlay a spinner
    /// </summary>
    public static class IsLoadingBehavior
    {
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.RegisterAttached(
            "IsLoading",
            typeof(bool),
            typeof(IsLoadingBehavior),
            new UIPropertyMetadata(false, OnIsLoadingChanged));

        public static bool GetIsLoading(DependencyObject d)
        {
            return (bool)d.GetValue(IsLoadingProperty);
        }

        public static void SetIsLoading(DependencyObject d, bool value)
        {
            d.SetValue(IsLoadingProperty, value);
        }

        private static void OnIsLoadingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = sender as FrameworkElement;
            // IF SENDER IS NOT A CONTROL, JUST EXIT
            if (control == null)
            {
                return;
            }

            bool isLoading = (bool)(e.NewValue);

            // DISABLE THE CONTROL
            control.IsEnabled = !isLoading;
            hookControl(control, isLoading);
        }

        private static void hookControl(FrameworkElement control, bool isLoading)
        {
            var parentAdorner = AdornerLayer.GetAdornerLayer(control);
            if (isLoading)
            {
                // REGISTER FOR UPDATES WHENEVER THE CONTROL IS UNLOADED
                control.AddHandler(FrameworkElement.UnloadedEvent, new RoutedEventHandler(control_Unloaded), true);
                var adorner = new SpinnerAdorner(control);
                parentAdorner.Add(adorner);
            }
            else
            {
                // UNREGISTER
                control.RemoveHandler(FrameworkElement.UnloadedEvent, new RoutedEventHandler(control_Unloaded));
                parentAdorner.Remove(parentAdorner.GetAdorners(control).FirstOrDefault());
            }

        }
        private static void control_Unloaded(object sender, RoutedEventArgs e)
        {
            var control = sender as FrameworkElement;
            hookControl(control, false);
        }

    }
}
