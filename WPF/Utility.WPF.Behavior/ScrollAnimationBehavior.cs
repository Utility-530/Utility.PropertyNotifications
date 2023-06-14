using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows;
using System.Windows.Input;

namespace Utility.WPF.Behavior
{
    public class ScrollAnimationBehavior : Behavior<ScrollViewer>
    {
        public static readonly DependencyProperty ItemMaxHeightProperty =
            DependencyProperty.Register("ItemMaxHeight", typeof(double), typeof(ScrollAnimationBehavior), new PropertyMetadata(100d));

        private bool mouseDown;

        public double ItemMaxHeight
        {
            get => (double)GetValue(ItemMaxHeightProperty);
            set => SetValue(ItemMaxHeightProperty, value);
        }

        protected override void OnAttached()
        {
            this.AssociatedObject.ScrollChanged += AssociatedObjectOnScrollChanged;
            this.AssociatedObject.PreviewMouseDown += AssociatedObject_MouseDown;
            this.AssociatedObject.MouseUp += AssociatedObject_MouseUp;
            base.OnAttached();
        }


        protected override void OnDetaching()
        {
            this.AssociatedObject.ScrollChanged -= AssociatedObjectOnScrollChanged;
            this.AssociatedObject.PreviewMouseDown -= AssociatedObject_MouseDown;
            base.OnDetaching();
        }

        private void AssociatedObject_MouseUp(object sender, MouseButtonEventArgs e)
        {
            mouseDown = false;
        }


        private void AssociatedObject_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            mouseDown = true;
        }

        private void AssociatedObjectOnScrollChanged(object _, ScrollChangedEventArgs e)
        {
            if (mouseDown == false)
                ScrollViewer_ScrollChanged(this.AssociatedObject, ItemMaxHeight, e);
        }

        private static void ScrollViewer_ScrollChanged(ScrollViewer scrollViewer, double itemMaxHeight, ScrollChangedEventArgs e)
        {

            const double MIN_SCALE = 0.0;
            const double MAX_SCALE = Math.PI;
            var viewHeight = scrollViewer.ActualHeight;

            foreach (FrameworkElement image in GetVisibleItems())
            {
                double currentPosition = GetElementPosition(image, scrollViewer.Parent as FrameworkElement).Y + image.ActualHeight / 2;
                double mappedHeightValue = Map(currentPosition,
                    itemMaxHeight * (-1), viewHeight + itemMaxHeight, MIN_SCALE, MAX_SCALE);

                var scale = Math.Sin(mappedHeightValue);

                DoubleAnimation heightAnimation = new DoubleAnimation
                    (image.ActualHeight, itemMaxHeight * scale,
                    TimeSpan.FromMilliseconds(10), FillBehavior.HoldEnd);
                image.BeginAnimation(FrameworkElement.HeightProperty, heightAnimation);
            }

            Point GetElementPosition(FrameworkElement childElement, FrameworkElement absoluteElement)
            {
                return childElement.TransformToAncestor(absoluteElement).Transform(new Point(0, 0));
            }

            IEnumerable<FrameworkElement> GetVisibleItems()
            {
                switch (scrollViewer.Content)
                {
                    case Panel panel:
                        {
                            foreach (FrameworkElement item in panel.Children)
                            {
                                if (IsItemVisible(item, scrollViewer))
                                    yield return item;
                            }

                            break;
                        }
                    case ItemsControl itemsControl:
                        {
                            foreach (object item in itemsControl.Items)
                            {
                                FrameworkElement element = item as FrameworkElement;
                                if (element is null)
                                    element = (FrameworkElement)itemsControl.ItemContainerGenerator.ContainerFromItem(item);
                                if (element is null)
                                    continue;
                                if (IsItemVisible(element, scrollViewer))
                                    yield return element;
                            }

                            break;
                        }
                    default:
                        throw new Exception("G  S$GBGdgflkfdg fdgdf");
                }


                static bool IsItemVisible(FrameworkElement child, FrameworkElement parent)
                {
                    var childTransform = child.TransformToAncestor(parent);
                    var childRectangle = childTransform.TransformBounds(new Rect(new Point(0, 0), child.RenderSize));
                    var ownerRectangle = new Rect(new Point(0, 0), parent.RenderSize);
                    return ownerRectangle.IntersectsWith(childRectangle);
                }
            }

            static double Map(double value, double fromMin, double fromMax, double toMin, double toMax)
            {
                return toMin + (value - fromMin) / (fromMax - fromMin) * (toMax - toMin);
            }
        }
    }
}
