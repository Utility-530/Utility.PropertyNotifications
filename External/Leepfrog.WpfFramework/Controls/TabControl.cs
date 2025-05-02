using Leepfrog.WpfFramework.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Leepfrog.WpfFramework.Controls
{
    public class TabControl2 : System.Windows.Controls.TabControl
    {
        static TabControl2()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabControl2), new FrameworkPropertyMetadata(typeof(TabControl2)));            
        }

        public bool IsInDesignMode { get; private set; }

        public RelayCommand BackCommand { get; private set; }
        public RelayCommand ClearHistoryCommand { get; private set; }
        public RelayCommand RemoveFromHistoryCommand { get; private set; }

        public TabControl2()
        {
            BackCommand = new RelayCommand(param => goBack(), param => History?.Any() ?? false);
            ClearHistoryCommand = new RelayCommand(param => clearHistory());
            RemoveFromHistoryCommand = new RelayCommand(param => removeFromHistory());
            IsInDesignMode = DesignerProperties.GetIsInDesignMode(this);
            History = new Stack<int>();
        }

        public Stack<int> History
        {
            get { return (Stack<int>)GetValue(HistoryProperty); }
            set { SetValue(HistoryProperty, value); }
        }

        // Using a DependencyProperty as the backing store for History.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HistoryProperty =
            DependencyProperty.Register("History", typeof(Stack<int>), typeof(TabControl2), new PropertyMetadata(null,History_Changed,History_Coerce));

        private static object History_Coerce(DependencyObject d, object baseValue)
        {
            var tabs = (d as TabControl2);
            // don't allow History to be set to null
            if (baseValue == null)
            {
                return tabs?.History;
            }
            return baseValue;
        }

        private static void History_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabControl2 tabs)
            {
                // don't do this if we're in design mode
                if (tabs.IsInDesignMode)
                {
                    return;
                }
                if (e.NewValue is Stack<int> newValue)
                {
                    if (newValue.Any())
                    {
                        var currentTab = newValue.Peek();
                        tabs.SelectWithoutHistoryByIndex(currentTab);
                    }
                }
            }
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            //e.Handled = true;
            if (!_skipAddToHistory)
            {
                foreach ( TabItem tab in e.RemovedItems )
                {
                    if ( GetIncludeInHistory(tab) )
                    {
                        var index = Items.IndexOf(tab);
                        // only add to history if it's different!
                        if (
                            (History?.Any() ?? false)
                         && (History?.Peek() == index)
                           )
                        {
                            // would be a duplicate, don't do it!
                        }
                        else
                        {
                            History?.Push(index);
                            logHistory("PUSH");
                        }
                    }
                }
                BackCommand.RaiseCanExecuteChanged();
            }
            base.OnSelectionChanged(e);
        }

        private void logHistory(string v)
        {
            if (History == null)
            {
                return;
            }
            var history = string.Join(",", History.Take(20)); // limit to most recent 20 entries, to avoid logging huge strings
            this.AddLog("TAB", $"HISTORY {v} -> { history}");
        }

        internal void SelectWithoutHistory(TabItem tab)
        {
            this.AddLog("TAB", $"SELECT WITHOUT HISTORY");
            // change this to pop history, instead of skip history!
            _skipAddToHistory = true;
            SelectedItem = tab;
            _skipAddToHistory = false;
        }

        internal void SelectWithoutHistoryByIndex(int index)
        {
            _skipAddToHistory = true;
            SelectedIndex = index;
            _skipAddToHistory = false;
        }

        private bool _skipAddToHistory = false;

        private void goBack()
        {
            int index = SelectedIndex;
            // keep popping, as long as we have history and we haven't found a different tab yet...
            while (
                   (History?.Any() ?? false)
                && (index == SelectedIndex)
                  )
            {
                index = History.Pop();
                logHistory("POP");
            }
            SelectWithoutHistoryByIndex(index);
            BackCommand.RaiseCanExecuteChanged();
        }

        private void removeFromHistory()
        {
            if ( History?.Any() ?? false )
            {
                History.Pop();
                logHistory("POP");
                BackCommand.RaiseCanExecuteChanged();
            }
        }

        private void clearHistory()
        {
            History?.Clear();
            BackCommand.RaiseCanExecuteChanged();
        }

        #region IncludeInHistory
        // ********************************************************************
        public static bool GetIncludeInHistory(DependencyObject obj) => (bool)obj.GetValue(IncludeInHistoryProperty);
        public static void SetIncludeInHistory(DependencyObject obj, bool value)
        {
            obj.SetValue(IncludeInHistoryProperty, value);
        }
        public static readonly DependencyProperty IncludeInHistoryProperty =
            DependencyProperty.RegisterAttached("IncludeInHistory", typeof(bool), typeof(TabControl2), new PropertyMetadata(true));
        // ********************************************************************
        #endregion
        #region IsBackAllowed
        // ********************************************************************
        public static bool GetIsBackAllowed(DependencyObject obj) => (bool)obj.GetValue(IsBackAllowedProperty);
        public static void SetIsBackAllowed(DependencyObject obj, bool value)
        {
            obj.SetValue(IsBackAllowedProperty, value);
        }
        public static readonly DependencyProperty IsBackAllowedProperty =
            DependencyProperty.RegisterAttached("IsBackAllowed", typeof(bool), typeof(TabControl2), new PropertyMetadata(true));
        // ********************************************************************
        #endregion
        #region Headings

        public static string GetHeading1(DependencyObject obj)
        {
            return (string)obj.GetValue(Heading1Property);
        }

        public static void SetHeading1(DependencyObject obj, string value)
        {
            obj.SetValue(Heading1Property, value);
        }

        public static string GetHeading2(DependencyObject obj)
        {
            return (string)obj.GetValue(Heading2Property);
        }

        public static void SetHeading2(DependencyObject obj, string value)
        {
            obj.SetValue(Heading2Property, value);
        }

        // Using a DependencyProperty as the backing store for Heading1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Heading1Property =
            DependencyProperty.RegisterAttached("Heading1", typeof(string), typeof(TabControl2), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty Heading2Property =
            DependencyProperty.RegisterAttached("Heading2", typeof(string), typeof(TabControl2), new PropertyMetadata(default(string)));
        #endregion


    }
}
