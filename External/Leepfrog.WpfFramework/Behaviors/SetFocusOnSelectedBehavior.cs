using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public class SetFocusOnSelectedBehavior
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
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(SetFocusOnSelectedBehavior), new PropertyMetadata(false,isEnabled_PropertyChanged));

        

        private static void isEnabled_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //-----------------------------------------------------------------
            var tabControl = d as TabControl;
            if (DesignerProperties.GetIsInDesignMode(tabControl))
            {
                return;
            }
            //-----------------------------------------------------------------
            tabControl.SelectionChanged += selChanged;
            //-----------------------------------------------------------------
        }

        private static void selChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(e.Source is TabControl))
            {
                return;
            }
            if ((e.AddedItems != null) && (e.AddedItems.Count > 0))
            {
                var i = e.AddedItems[0] as TabItem;
                if (i == null)
                {
                    return;
                }
                var p = i.Content as UIElement;
                if (p==null)
                {
                    return;
                }
                p.Dispatcher.BeginInvoke(new Action(
                    () =>
                    {
                        var c = GetLeafFocusableChild(p) as UIElement;
                        if (c != null)
                        {
                            c.Focus();
                        }
                    }), DispatcherPriority.Loaded);
            }
        }

        /// <summary>
        /// Locate the first real focusable child.  We keep going down
        /// the visual tree until we hit a leaf node.
        /// </summary>
        /// <param name="fe"></param>
        /// <returns></returns>
        static IInputElement GetLeafFocusableChild(IInputElement fe)
        {
            IInputElement ie = GetFirstFocusableChild(fe), final = ie;
            while (final != null)
            {
                ie = final;
                final = GetFirstFocusableChild(final);
            }

            return ie;
        }

        /// <summary>
        /// This searches the Visual Tree looking for a valid child which can have focus.
        /// </summary>
        /// <param name="fe"></param>
        /// <returns></returns>
        static IInputElement GetFirstFocusableChild(IInputElement fe)
        {
            var dpo = fe as DependencyObject;
            return dpo == null ? null : (from vc in EnumerateVisualTree(dpo, c => !FocusManager.GetIsFocusScope(c))
                                         let iic = vc as IInputElement
                                         where iic != null && iic.Focusable && iic.IsEnabled &&
                                         (!(iic is FrameworkElement) || (((FrameworkElement)iic).IsVisible))
                                         select iic).FirstOrDefault();
        }

        /// <summary>
        /// A simple iterator method to expose the visual tree to LINQ
        /// </summary>
        /// <param name="start"></param>
        /// <param name="eval"></param>
        /// <returns></returns>
        static IEnumerable<T> EnumerateVisualTree<T>(T start, Predicate<T> eval) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(start); i++)
            {
                var child = VisualTreeHelper.GetChild(start, i) as T;
                if (child != null && (eval != null && eval(child)))
                {
                    yield return child;
                    foreach (var childOfChild in EnumerateVisualTree(child, eval))
                        yield return childOfChild;
                }
            }
        }
    }
}
