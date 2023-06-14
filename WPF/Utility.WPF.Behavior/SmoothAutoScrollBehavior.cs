using System.Collections;
using System.Threading.Tasks;

namespace Utility.WPF.Behavior
{
    using Microsoft.Xaml.Behaviors;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Animation;


    public enum ScrollablePosition
    {
        Start, Selected, End,
        Middle
    }


    /// <summary>
    /// Uses ItemsControl's internal scrollviewer to move last item into view when items collection modified.
    /// <a href="https://social.msdn.microsoft.com/Forums/vstudio/en-US/8a745550-4360-4b53-85f5-032f8f46e2cc/listview-scrollintoview-not-working-with-a-observablecollection?forum=wpf"></a>
    /// <a href="https://stackoverflow.com/questions/16942580/why-doesnt-listview-scrollintoview-ever-work"></a>
    /// <a href="https://stackoverflow.com/questions/7153302/scroll-animation"></a>
    /// </summary>
    public class SmoothAutoScrollBehavior : Behavior<ItemsControl>
    {
        public static readonly DependencyProperty WithAnimationProperty =
            DependencyProperty.Register("WithAnimation", typeof(bool), typeof(SmoothAutoScrollBehavior), new PropertyMetadata(true));


        public static readonly DependencyProperty ScrollablePositionProperty =
            DependencyProperty.Register("ScrollablePosition", typeof(ScrollablePosition), typeof(SmoothAutoScrollBehavior),
                new PropertyMetadata(ScrollablePosition.Selected));

        private ScrollViewer? scrollViewer;
        private Storyboard? storyboard;

        #region properties
        public ScrollablePosition ScrollablePosition
        {
            get => (ScrollablePosition)GetValue(ScrollablePositionProperty);
            set => SetValue(ScrollablePositionProperty, value);
        }


        public bool WithAnimation
        {
            get => (bool)GetValue(WithAnimationProperty);
            set => SetValue(WithAnimationProperty, value);
        }
        #endregion properties

        protected override void OnAttached()
        {
            if (AssociatedObject.Items is INotifyCollectionChanged notifyCollectionChanged)
                notifyCollectionChanged.CollectionChanged += ListBox_CollectionChanged;
            if (AssociatedObject is ListBox listBox)
            {
                listBox.SelectionChanged += ListBox_SelectionChanged;
            }
            base.OnAttached();
        }

        private async void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var change = e.ExtentHeightChange;
            if (change < 0)
            {
                if (AssociatedObject is ListBox listBox)
                {
                    //await Task.Delay(1000);
                    //listBox.SelectedIndex--;
                }
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Run();
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject.Items is INotifyCollectionChanged notifyCollectionChanged)
                notifyCollectionChanged.CollectionChanged -= ListBox_CollectionChanged;
            base.OnDetaching();
        }

        private void ListBox_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action is NotifyCollectionChangedAction.Add or NotifyCollectionChangedAction.Remove)
            {
                try
                {
                    Run();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error updating list view", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void Run()
        {
            storyboard?.SkipToFill();

            if (scrollViewer == null)
            {
                scrollViewer =
                    Helper.GetDescendantByType(this.AssociatedObject, typeof(ScrollViewer)) as ScrollViewer ??
                    throw new Exception("sddg4 444 f");
                scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
            }
            //if (WithAnimation)

            //{
            double position = 0;
            switch (ScrollablePosition)
            {
                case ScrollablePosition.Start:
                    position = 1;
                    break;
                case ScrollablePosition.Middle:
                    position = this.AssociatedObject.Items.Count / 2d;
                    break;
                case ScrollablePosition.Selected:
                    if (AssociatedObject is ListBox listBox)
                        position = listBox.SelectedIndex;
                    else
                    {
                        throw new Exception("SDG$£B  FG5555");
                    }
                    break;
                case ScrollablePosition.End:
                    position = this.AssociatedObject.Items.Count;
                    break;

            }

            await Task.Delay(300);
            if (WithAnimation)
            {
                storyboard =
                    Helper.MakeScrollAnimation(scrollViewer, Helper.GetRatio(this.AssociatedObject, position), 0);
                // Add delay to make things appear smoother

                storyboard.Begin();
            }
            else
                scrollViewer.ScrollToVerticalOffset(scrollViewer.ScrollableHeight * position);
        }

        private static class Helper
        {


            public static Storyboard MakeScrollAnimation(ScrollViewer scrollViewer, double ratio, double offset)
            {
                double toValue = scrollViewer.ScrollableHeight * ratio;
                DoubleAnimation verticalAnimation = new()
                {
                    From = scrollViewer.VerticalOffset,
                    To = toValue + offset,
                    Duration = new Duration(TimeSpan.FromMilliseconds(500))
                };
                Storyboard storyboard = new Storyboard();

                storyboard.Children.Add(verticalAnimation);
                Storyboard.SetTarget(verticalAnimation, scrollViewer);
                Storyboard.SetTargetProperty(verticalAnimation, new PropertyPath(ScrollViewerBehavior.VerticalOffsetProperty));
                return storyboard;
            }

            public static double GetRatio(ItemsControl itemsControl, double index)
            {
                return index / itemsControl.Items.Count;
            }

            public static Visual? GetDescendantByType(Visual? element, Type type)
            {
                if (element == null) return null;
                if (element.GetType() == type) return element;
                Visual? foundElement = null;
                if (element is FrameworkElement frameworkElement)
                {
                    frameworkElement.ApplyTemplate();
                }

                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
                {
                    if (VisualTreeHelper.GetChild(element, i) is Visual visual)
                    {
                        foundElement = GetDescendantByType(visual, type);
                        if (foundElement != null)
                            break;
                    }
                }

                return foundElement;
            }
        }
    }

    public static class EnumerableHelper
    {

        public static int IndexOf(this IEnumerable source, object value)
        {
            int index = 0;
            var comparer = EqualityComparer<object>.Default; // or pass in as a parameter
            foreach (object item in source)
            {
                if (comparer.Equals(item, value)) return index;
                index++;
            }

            return -1;
        }
    }

    /// <summary>
    ///   Based on the following link
    ///   <a href="http://aniscrollviewer.codeplex.com/"></a>
    ///  The VerticalOffset property is read-only so instead you can use an attached property VerticalOffset on the ScrollViewer which in turn does ScrollToVerticalOffset.This attached property can be animated.
    /// </summary>
    public class ScrollViewerBehavior
    {
        public static DependencyProperty VerticalOffsetProperty =
            DependencyProperty.RegisterAttached("VerticalOffset",
                                                typeof(double),
                                                typeof(ScrollViewerBehavior),
                                                new UIPropertyMetadata(0d, OnVerticalOffsetChanged));

        public static void SetVerticalOffset(FrameworkElement target, double value)
        {
            target.SetValue(VerticalOffsetProperty, value);
        }

        public static double GetVerticalOffset(FrameworkElement target)
        {
            return (double)target.GetValue(VerticalOffsetProperty);
        }

        private static void OnVerticalOffsetChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (target is ScrollViewer scrollViewer)
            {
                scrollViewer.ScrollToVerticalOffset((double)e.NewValue);
            }
        }
    }
}