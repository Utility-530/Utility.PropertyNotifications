/*
 * Author:  Leif Goodwin
 * Date:    2017-01-17
 * Purpose: Defines an attached behaviour which adds additional formatting
 *          options to the DataGrid control.
 *
 *              1) Configure shared size group columns (as per the WPF Grid
 *                 sharedsizegroup property).
 *              2) Shrink the columns to fit their content.
 *              3) Enable/disable the formatting.
 *              <a href="https://www.codeproject.com/Articles/1166016/WPF-DataGrid-with-SharedSizeGroup-Columns-Property"/>
 */

using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace Utility.WPF.Controls.DataGrids
{
    public class ShareWidthsBehavior : Behavior<DataGrid>
    {
        public static readonly DependencyProperty MarginProperty =
            DependencyProperty.Register("Margin", typeof(double), typeof(ShareWidthsBehavior), new PropertyMetadata(0d));

        private FrameworkElement? parent;

        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject.Parent is FrameworkElement _parent)
            {
                this.parent = _parent;
            }
            else
            {
                var templatedParent = (AssociatedObject.TemplatedParent as FrameworkElement);
                while (templatedParent.Parent as FrameworkElement == null)
                {
                    templatedParent = templatedParent.TemplatedParent as FrameworkElement;
                }
                this.parent = templatedParent.Parent as FrameworkElement;
            }
            if (parent == null)
                throw new Exception("fd ddd445");

            if (parent.IsLoaded)
            {
                ShareWidthsBehavior_Loaded(default, default);
            }
            else
                parent.Loaded += ShareWidthsBehavior_Loaded;
        }

        public double Margin
        {
            get { return (double)GetValue(MarginProperty); }
            set { SetValue(MarginProperty, value); }
        }

        private void ShareWidthsBehavior_Loaded(object sender, RoutedEventArgs e)
        {
            DataGridExtensionMethods.AdjustColumnWidths(AssociatedObject, parent, Margin);
            parent.SizeChanged += ShareWidthsBehavior_SizeChanged;
        }

        private void ShareWidthsBehavior_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var size = e.NewSize;
            DataGridExtensionMethods.AdjustColumnWidths(AssociatedObject, parent, Margin);
        }
    }
}