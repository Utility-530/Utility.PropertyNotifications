using System;
using System.Windows;
using System.Windows.Controls;
using ReactiveUI;
using Utility.WPF.Behaviors;

namespace Utility.WPF.Controls.Adorners
{
    public class SettingsControl : Control
    {
        public delegate void CheckedRoutedEventHandler(object sender, CheckedEventArgs e);

        public class CheckedEventArgs : RoutedEventArgs
        {
            public CheckedEventArgs(CheckedType checkedType, bool isChecked, RoutedEvent routedEvent, object source) : base(routedEvent, source)
            {
                CheckedType = checkedType;
                IsChecked = isChecked;
            }

            public CheckedType CheckedType { get; }
            public bool IsChecked { get; }
        }

        public enum CheckedType
        {
            DataContext,
            Dimensions,
            HighlightArea
        }

        public static readonly RoutedEvent CheckedEvent = EventManager.RegisterRoutedEvent("Checked", RoutingStrategy.Bubble, typeof(CheckedRoutedEventHandler), typeof(SettingsControl));

        public static readonly DependencyProperty SelectedDockProperty = DependencyProperty.Register("SelectedDock", typeof(Dock?), typeof(SettingsControl), new PropertyMetadata(null, Change /*m,coerceValueCallback: Callback*/));

        private MenuItem? dimensionsMenuItem, dataContextMenuItem, highlightColourMenuItem;

        static SettingsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SettingsControl), new FrameworkPropertyMetadata(typeof(SettingsControl)));
        }

        public Dock? SelectedDock
        {
            get => (Dock?)GetValue(SelectedDockProperty);
            set => SetValue(SelectedDockProperty, value);
        }

        public event CheckedRoutedEventHandler Checked
        {
            add => AddHandler(CheckedEvent, value);
            remove => RemoveHandler(CheckedEvent, value);
        }

        public override void OnApplyTemplate()
        {
            MenuItemButtonGroupBehavior? behavior = GetTemplateChild("MenuItemButtonGroupBehavior") as MenuItemButtonGroupBehavior;
            behavior
                .WhenAnyValue(a => a.SelectedItem)
                .Subscribe(a =>
                {
                    if (a is null)
                    {
                        return;
                    }

                    if (a is EnumMember { Value: null })
                    {
                        SelectedDock = null;
                        return;
                    }
                    if (a is EnumMember { Value: Dock value })
                    {
                        SelectedDock = value;
                        return;
                    }

                    throw new Exception("fd   99f");
                });

            //<MenuItem x:Name="HeightMenuItem"  Header="Height" IsCheckable="True"></MenuItem>
            //                  <MenuItem x:Name="DataContextMenuItem"  Header="DataContext" IsCheckable="True"></MenuItem>

            dimensionsMenuItem = GetTemplateChild("DimensionsMenuItem") as MenuItem;
            dimensionsMenuItem.Checked += DimensionsMenuItem_Checked;
            dimensionsMenuItem.Unchecked += DimensionsMenuItem_Checked;

            dataContextMenuItem = GetTemplateChild("DataContextMenuItem") as MenuItem;
            dataContextMenuItem.Checked += DataContextMenuItem_Checked;
            dataContextMenuItem.Unchecked += DataContextMenuItem_Checked;

            highlightColourMenuItem = GetTemplateChild("HighlightColourMenuItem") as MenuItem;
            highlightColourMenuItem.Checked += HighlightMenuItem_Checked;
            highlightColourMenuItem.Unchecked += HighlightMenuItem_Checked;

            base.OnApplyTemplate();
        }

        private static void Change(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private void DataContextMenuItem_Checked(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new CheckedEventArgs(CheckedType.DataContext, dataContextMenuItem.IsChecked, CheckedEvent, this));
        }

        private void DimensionsMenuItem_Checked(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new CheckedEventArgs(CheckedType.Dimensions, dimensionsMenuItem.IsChecked, CheckedEvent, this));
        }

        private void HighlightMenuItem_Checked(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new CheckedEventArgs(CheckedType.HighlightArea, highlightColourMenuItem.IsChecked, CheckedEvent, this));
        }

        //private static object Callback(DependencyObject d, object baseValue)
        //{
        //    if (baseValue is EnumMember { Value: var value } member)
        //    {
        //        return value;
        //    }
        //    throw new Exception("DGF£FGVV vv446");
        //}
    }
}