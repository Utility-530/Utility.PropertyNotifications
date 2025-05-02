using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using Leepfrog.WpfFramework.Controls;


namespace Leepfrog.WpfFramework.Actions
{
    /// <summary>
    /// Trigger action to set focus to another control
    /// </summary>
    public class SelectTabAction : TriggerAction<UIElement>
    {
        public TabItem Tab
        {
            get { return (TabItem)GetValue(TabProperty); }
            set { SetValue(TabProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Tab.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TabProperty =
            DependencyProperty.Register("Tab", typeof(TabItem), typeof(SelectTabAction), new PropertyMetadata(null));




        public bool SkipHistory
        {
            get { return (bool)GetValue(SkipHistoryProperty); }
            set { SetValue(SkipHistoryProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SkipHistory.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SkipHistoryProperty =
            DependencyProperty.Register("SkipHistory", typeof(bool), typeof(SelectTabAction), new PropertyMetadata(false));


        protected override void Invoke(object parameter)
        {
            if (Tab == null)
            {
                return;
            }
            if (SkipHistory)
            {
                this.AddLog("TAB", "SKIP HISTORY IS SET...");
            }
            if (Tab.Parent is TabControl tabControl)
            { 
                if (
                    (tabControl is TabControl2 tc2)
                 && (SkipHistory)
                   )
                {
                    this.AddLog("TAB", "SKIP HISTORY IS SET... SKIPPING");
                    tc2.SelectWithoutHistory(Tab);
                }
                else
                {
                    this.AddLog("TAB", "FORCE SELECT TAB");
                    tabControl.SelectedItem = Tab;
                }
            }
        }
    }
}
