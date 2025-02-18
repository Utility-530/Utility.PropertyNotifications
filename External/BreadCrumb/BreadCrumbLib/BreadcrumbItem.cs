using BreadcrumbLib.Infrastructure;
using SniffCore.Buttons;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BreadcrumbLib
{
    public class BreadcrumbItem : SelectableHeaderedItemsControl
    {

        private static RoutedUICommand invokeMenuCommand = new RoutedUICommand("Open menu", "InvokeMenuCommand", typeof(BreadcrumbItem));
        private SplitButton button;

        public static RoutedUICommand InvokeMenuCommand => invokeMenuCommand;


        static BreadcrumbItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BreadcrumbItem), new FrameworkPropertyMetadata(typeof(BreadcrumbItem)));
        }

        public BreadcrumbItem()
            : this(null)
        { }

        internal BreadcrumbItem(object parent) : base(parent)
        {
            CommandBindings.Add(new CommandBinding(InvokeMenuCommand, CommandBindingToMenuItem_Executed));
        }

        public override void OnApplyTemplate()
        {
            button = this.GetTemplateChild("button") as SplitButton;
            button.SizeChanged += Button_SizeChanged;
            base.OnApplyTemplate();
        }

        private void Button_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Move the popup to the bottom-right corner of the BreadcrumbItem
            button.PlacementRectangle = new Rect(e.NewSize.Width, e.NewSize.Height, 0, 0);
        }

        private void CommandBindingToMenuItem_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var breadcrumb = VisualTreHelperEx.FindVisualParent<Breadcrumb>(this);
            breadcrumb.AddTrail(ParentItem, e.Parameter);
        }
    }
}