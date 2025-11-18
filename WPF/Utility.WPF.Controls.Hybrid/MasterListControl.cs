using System;
using System.Collections;
using System.Reactive.Linq;
using System.Windows;
using Utility.WPF.Reactives;
using Utility.WPF.Controls.Dragablz;
using Utility.WPF.Controls.Master;

namespace Utility.WPF.Controls.Hybrid
{
    public class MasterListControl : MasterBindableControl
    {
        public static readonly DependencyProperty CommandPathProperty = DependencyProperty.Register("CommandPath", typeof(string), typeof(MasterListControl), new PropertyMetadata(null));
        public static readonly DependencyProperty IsCheckedPathProperty = DependencyProperty.Register("IsCheckedPath", typeof(string), typeof(MasterListControl), new PropertyMetadata(null));
        public static readonly DependencyProperty IsRefreshablePathProperty = DependencyProperty.Register("IsRefreshablePath", typeof(string), typeof(MasterListControl), new PropertyMetadata(null));

        static MasterListControl()
        {
            // FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterTicksControl), new FrameworkPropertyMetadata(typeof(MasterTicksControl)));
        }

        public MasterListControl()
        {
            Movement = Enums.XYTraversal.TopToBottom;
            RemoveOrder = RemoveOrder.Selected;
            ButtonTypes = ButtonType.Add | ButtonType.Remove;

            this.Observe(a => a.ItemsSource)
                .StartWith(default(IEnumerable))
           .CombineLatest(
           this.Observe(a => a.DisplayMemberPath),
           this.Observe(a => a.IsCheckedPath),
           this.Observe(a => a.CommandPath),
           this.Observe(a => a.IsRefreshablePath)
           )
           .Subscribe(a =>
           {
               var (itemsSource, display, isChecked, commandPath, isRefreshable) = a;
               //this.Dispatcher.InvokeAsync(() =>
               //{
               if ((Content ??= new ListControl()) is ListControl msn)
               {
                   msn.ItemsSource = itemsSource;
                   msn.IsCheckedPath = isChecked;
                   msn.CommandPath = commandPath;
                   msn.DisplayMemberPath = display;
                   msn.IsRefreshablePath = isRefreshable;
               }
               else
               {
                   throw new ApplicationException("Expected Content to be " + nameof(ListControl));
               }
               //});
           });
        }

        public string IsCheckedPath
        {
            get => (string)GetValue(IsCheckedPathProperty);
            set => SetValue(IsCheckedPathProperty, value);
        }

        public string CommandPath
        {
            get => (string)GetValue(CommandPathProperty);
            set => SetValue(CommandPathProperty, value);
        }

        public string IsRefreshablePath
        {
            get { return (string)GetValue(IsRefreshablePathProperty); }
            set { SetValue(IsRefreshablePathProperty, value); }
        }
    }
}