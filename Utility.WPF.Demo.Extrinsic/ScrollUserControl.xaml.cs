using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using UappUI.AppCode.Touch;

namespace Utility.WPF.Demo.Extrinsic
{
    /// <summary>
    /// Interaction logic for ScrollUserControl.xaml
    /// </summary>
    public partial class ScrollUserControl : UserControl
    {
        public ScrollUserControl()
        {
            InitializeComponent();

            var dates = Enumerable.Range(-15, 30).Select(a => DateTime.Now.AddDays(a));
            DatesListBox.ItemsSource = dates;
            //Dates2ListBox.ItemsSource = dates;
        }


        //private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        //{
        //    if (!(sender is ScrollViewer scrollViewer))
        //        return;

        //    const double MIN_SCALE = 0.0;
        //    const double MAX_SCALE = Math.PI;
        //    var viewHeight = scrollViewer.ActualHeight;
        //    var itemMaxHeight = 100;

        //    foreach (FrameworkElement image in GetVisibleItems())
        //    {
        //        double currentPosition = GetElementPosition(image, scrollViewer.Parent as FrameworkElement).Y + image.ActualHeight / 2;
        //        double mappedHeightValue = currentPosition.Map(
        //            itemMaxHeight * (-1), viewHeight + itemMaxHeight, MIN_SCALE, MAX_SCALE);

        //        var scale = Math.Sin(mappedHeightValue);

        //        DoubleAnimation heightAnimation = new DoubleAnimation
        //            (image.ActualHeight, itemMaxHeight * scale,
        //            TimeSpan.FromMilliseconds(10), FillBehavior.HoldEnd);
        //        image.BeginAnimation(HeightProperty, heightAnimation);
        //    }

        //    Point GetElementPosition(FrameworkElement childElement, FrameworkElement absoluteElement)
        //    {
        //        return childElement.TransformToAncestor(absoluteElement).Transform(new Point(0, 0));
        //    }

        //    IEnumerable<FrameworkElement> GetVisibleItems()
        //    {
        //        StackPanel container = (StackPanel)scrollViewer.Content;

        //        foreach (FrameworkElement item in container.Children)
        //            if (IsItemVisible(item, scrollViewer))
        //                yield return (item);

        //        bool IsItemVisible(FrameworkElement child, FrameworkElement parent)
        //        {
        //            var childTransform = child.TransformToAncestor(scrollViewer);
        //            var childRectangle = childTransform.TransformBounds(new Rect(new Point(0, 0), child.RenderSize));
        //            var ownerRectangle = new Rect(new Point(0, 0), scrollViewer.RenderSize);
        //            return ownerRectangle.IntersectsWith(childRectangle);
        //        }
        //    }
        //}
    }
}