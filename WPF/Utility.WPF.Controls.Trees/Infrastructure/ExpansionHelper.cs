using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Utility.WPF.Controls.Trees.Infrastructure;

namespace Utility.WPF.Controls.Trees
{
    public static class ExpansionHelper
    {
        public static readonly DependencyProperty IsExpandAllEnabledProperty =
    DependencyProperty.RegisterAttached(
        "IsExpandAllEnabled",
        typeof(bool),
        typeof(ExpansionHelper),
        new PropertyMetadata(false, changed));

        private static void changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is TreeViewItem item)
            {
                if((bool)e.NewValue)
                {
                    CommandBinding binding = new CommandBinding(ExpandAllCommand, (s, ev) =>
                    {
                        ExpansionHelper.ExpandAll(item);
                    });
                    item.CommandBindings.Add(binding);
                }
                else
                {
                    var binding = item.CommandBindings.OfType<CommandBinding>().FirstOrDefault(a => a.Command == ExpandAllCommand);
                    if(binding != null)
                    {
                        item.CommandBindings.Remove(binding);
                    }
                }
            }
        }

        private static void ExpandAll(TreeViewItem item)
        {
            Utility.WPF.Helpers.TreeViewHelper.ExpandAllWithTracking(item);
        }

        public static bool GetIsExpandAllEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsExpandAllEnabledProperty);
        }

        public static void SetIsExpandAllEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsExpandAllEnabledProperty, value);
        }


        /// <summary>
        /// Routed command which can be used to close a tab.
        /// </summary>
        public static RoutedCommand ExpandAllCommand = new RoutedUICommand("ExpandAll", "ExpandAll", typeof(TreeViewItem));

    }
}
