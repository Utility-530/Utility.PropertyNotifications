using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Media;
using System.Globalization;
using Leepfrog.WpfFramework.Commands;
using Microsoft.Xaml.Behaviors;

namespace Leepfrog.WpfFramework.Behaviors
{
    /// <summary>
    /// Behaviour to be applied to Button controls
    /// Sets Command to SelectTabCommand, and CommandParameter to given Tab
    /// </summary>
    public static class SelectTabCommandBehavior
    {
        public static readonly DependencyProperty TabProperty =
            DependencyProperty.RegisterAttached(
            "Tab",
            typeof(TabItem),
            typeof(SelectTabCommandBehavior),
            new UIPropertyMetadata(null, OnTabChanged));

        public static TabItem GetTab(DependencyObject d)
        {
            return (TabItem)d.GetValue(TabProperty);
        }

        public static void SetTab(DependencyObject d, TabItem value)
        {
            d.SetValue(TabProperty, value);
        }
        private static void OnTabChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is Button b)
            {
                b.Command = SelectTabCommand;
                b.CommandParameter = (e.NewValue);
            }
            else if (sender is InvokeCommandAction a)
            {
                a.Command = SelectTabCommand;
                a.CommandParameter = (e.NewValue);
            }
        }

        #region SelectTabCommand
        public static RelayCommand SelectTabCommand { get; private set; }

        static SelectTabCommandBehavior()
        {
            SelectTabCommand = new RelayCommand(param => selectTab(param as TabItem));
        }

        private static void selectTab(TabItem tab)
        {
            if (tab?.Parent is TabControl tabControl)
            {
                tabControl.SelectedItem = tab;
            }
        }
        #endregion
    }
}
