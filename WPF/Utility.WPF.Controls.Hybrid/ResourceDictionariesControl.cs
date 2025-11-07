using System;
using System.Reactive.Linq;
using System.Windows;
using ReactiveUI;
using Utility.WPF.Controls.Dragablz;

namespace Utility.WPF.Controls.Hybrid
{
    public class ResourceDictionariesControl : MasterBindableControl
    {
        public static readonly DependencyProperty CommandPathProperty =
DependencyProperty.Register("CommandPath", typeof(string), typeof(ResourceDictionariesControl), new PropertyMetadata(null));

        public static readonly DependencyProperty IsCheckedPathProperty = DependencyProperty.Register("IsCheckedPath", typeof(string), typeof(ResourceDictionariesControl), new PropertyMetadata(null));

        static ResourceDictionariesControl()
        {
            // FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterTicksControl), new FrameworkPropertyMetadata(typeof(MasterTicksControl)));
        }

        public ResourceDictionariesControl()
        {
            ButtonTypes = ButtonType.None;
            this.WhenAnyValue(a => a.ItemsSource)
           .CombineLatest(this.WhenAnyValue(a => a.DisplayMemberPath), this.WhenAnyValue(a => a.IsCheckedPath), this.WhenAnyValue(a => a.CommandPath))
           .Subscribe(a =>
           {
               var (itemsSource, display, isChecked, commandPath) = a;
               //this.Dispatcher.InvokeAsync(() =>
               //{
               if ((Content ??= new ListControl()) is ListControl msn)
               {
                   msn.ItemsSource = itemsSource;
                   msn.CommandPath = commandPath;
                   msn.IsCheckedPath = isChecked;
                   msn.DisplayMemberPath = display;
               }
               else
               {
                   throw new ApplicationException("Expected Content to be " + nameof(ListControl));
               }
               //});
           });
        }

        public string CommandPath
        {
            get => (string)GetValue(CommandPathProperty);
            set => SetValue(CommandPathProperty, value);
        }

        public string IsCheckedPath
        {
            get => (string)GetValue(IsCheckedPathProperty);
            set => SetValue(IsCheckedPathProperty, value);
        }
    }
}