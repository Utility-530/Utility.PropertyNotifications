using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Linq;

namespace Leepfrog.WpfFramework.Behaviors
{
    /// <summary>
    /// Helper class for watching for lost focus
    /// </summary>
    public static class WatchFocusBehavior 
    {

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(WatchFocusBehavior),
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
            var control = (UIElement)sender;

            bool isEnabled = (bool)(e.NewValue);
            if (isEnabled == (bool)(e.OldValue))
            {
                return;
            }

            if (isEnabled)
            {
                control.LostFocus += control_lostFocus;
            }
            else
            {
                control.LostFocus -= control_lostFocus;
            }

        }

        private static void control_lostFocus(object sender, RoutedEventArgs e)
        {
            var control = (UIElement)sender;
            control.SetValue(WatchFocusBehavior.HasHadFocusProperty, true);
            control.LostFocus -= control_lostFocus;
        }

        public static readonly DependencyProperty HasHadFocusProperty =
            DependencyProperty.RegisterAttached(
            "HasHadFocus",
            typeof(bool),
            typeof(WatchFocusBehavior),
            new UIPropertyMetadata(false));

        public static bool GetHasHadFocus(DependencyObject d)
        {
            return (bool)d.GetValue(HasHadFocusProperty);
        }

        public static void SetHasHadFocus(DependencyObject d, bool value)
        {
            d.SetValue(HasHadFocusProperty, value);
        }
    }
}
