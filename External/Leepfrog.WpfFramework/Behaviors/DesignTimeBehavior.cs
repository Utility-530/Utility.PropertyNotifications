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
using System.Windows.Media;

namespace Leepfrog.WpfFramework.Behaviors
{
    /// <summary>
    /// Behaviour to be applied to any control
    /// When we're in design mode, change the background
    /// </summary>
    public static class DesignTimeBehavior
    {

        // Using a DependencyProperty as the backing store for Background.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.RegisterAttached(
                "Background", 
                typeof(Brush), 
                typeof(DesignTimeBehavior), 
                new UIPropertyMetadata(null, OnBackgroundChanged));


        public static Brush GetBackground(DependencyObject d)
        {
            return (Brush)d.GetValue(BackgroundProperty);
        }

        public static void SetBackground(DependencyObject d, Brush value)
        {
            d.SetValue(BackgroundProperty, value);
        }

        private static void OnBackgroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            // IF SENDER IS NOT A USERCONTROL, JUST EXIT
            if (!(sender is UserControl control))
            {
                return;
            }

            // if we're in design mode, set the background
            if (DesignerProperties.GetIsInDesignMode(control))
            {
                control.SetValue(Panel.BackgroundProperty, e.NewValue);
            }
        }
    }
}
