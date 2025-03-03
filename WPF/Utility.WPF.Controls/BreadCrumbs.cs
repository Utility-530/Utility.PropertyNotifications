using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.WPF.Controls.Base;

namespace Utility.WPF.Controls
{
    public class BreadCrumbs : CustomItemsControl
    {

        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(BreadCrumbs), new PropertyMetadata());
        public static readonly DependencyProperty ContainerWidthProperty =
DependencyProperty.Register("ContainerWidth", typeof(double), typeof(BreadCrumbs), new PropertyMetadata(80.0));
        static BreadCrumbs()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BreadCrumbs), new FrameworkPropertyMetadata(typeof(BreadCrumbs)));
            HorizontalAlignmentProperty.OverrideMetadata(typeof(BreadCrumbs), new FrameworkPropertyMetadata(HorizontalAlignment.Right));
        }

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        public double ContainerWidth
        {
            get { return (double)GetValue(ContainerWidthProperty); }
            set { SetValue(ContainerWidthProperty, value); }
        }

    }

    public class LastItemConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is ItemsControl itemscontrol && (values[1] is FrameworkElement fe))
            {
                int count = itemscontrol.Items.Count;

                if (values != null && values.Length >= 3 && count > 0)
                {
                    var lastItem = itemscontrol.Items[count - 1];
                    if (Equals(values[2], false) && Equals(lastItem, fe.DataContext) == false)
                    {
                        return true;
                    }
                    return false;
                }
            }

            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ScrollToBottomBehavior : Behavior<System.Windows.Controls.ScrollViewer>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            // Add event handler to scroll to the bottom when content changes
            AssociatedObject.LayoutUpdated += ScrollToBottom;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            // Remove event handler
            AssociatedObject.LayoutUpdated -= ScrollToBottom;
        }

        private void ScrollToBottom(object sender, System.EventArgs e)
        {
            AssociatedObject.ScrollToBottom();
        }
    }
}


