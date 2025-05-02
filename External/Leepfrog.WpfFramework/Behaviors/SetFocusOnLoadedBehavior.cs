using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Leepfrog.WpfFramework.Behaviors
{
    public class SetFocusOnLoadedBehavior
    {

        /// <summary>
        /// Sets focus to the first available child on a tabcontrol
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>


        public static bool GetIsEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsEnabledProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(SetFocusOnLoadedBehavior), new PropertyMetadata(false,isEnabled_PropertyChanged));

        

        private static void isEnabled_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //-----------------------------------------------------------------
            var element = d as FrameworkElement;
            element.Loaded += Element_Loaded; 
            //-----------------------------------------------------------------
        }

        private static void Element_Loaded(object sender, RoutedEventArgs e)
        {
            var element = sender as FrameworkElement;
            element.Focus();
        }

    }
}
