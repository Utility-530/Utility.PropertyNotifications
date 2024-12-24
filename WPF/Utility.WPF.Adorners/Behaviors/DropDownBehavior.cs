using Microsoft.Xaml.Behaviors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Utility.Helpers.NonGeneric;
using Utility.WPF.Adorners.Infrastructure;
using Utility.WPF.Controls;
using Utility.WPF.Reactives;

namespace Utility.WPF.Adorners.Behaviors
{

    public class DropDownBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(DropDownBehavior), new PropertyMetadata());
        public static readonly DependencyProperty IsShownProperty = DependencyProperty.Register("IsShown", typeof(bool), typeof(DropDownBehavior), new PropertyMetadata(ShowChanged));

        public bool IsShown
        {
            get { return (bool)GetValue(IsShownProperty); }
            set { SetValue(IsShownProperty, value); }
        }

        private static void ShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DropDownBehavior { AssociatedObject: { } associatedObject } behavior && e.NewValue.Equals(true))
            {
                behavior.Show(associatedObject);
            }
        }

        protected override void OnAttached()
        {
            if (IsShown)
                Show(AssociatedObject);
            else
                Hide(AssociatedObject);
            base.OnAttached();
        }

        void Show(FrameworkElement treeViewItem)
        {
            //bool b = false;
            if (treeViewItem.Adorners() == null)
            {
                //b = true;
                if (treeViewItem.GetValue(AdornerEx.AdornersProperty) == null)
                {
                    AddAdorner(treeViewItem, Make(ItemsSource));
                }
            }
            if (treeViewItem.GetValue(AdornerEx.IsEnabledProperty).Equals(false))
            {
                //b = true;
                try
                {
                    treeViewItem.SetValue(AdornerEx.IsEnabledProperty, true);
                }
                catch (Exception ex)
                {
                    IsShown = false;
                }
            }
            //if (b)
            //    Add(treeViewItem);

        }

        void Hide(FrameworkElement frameworkElement)
        {
            IsShown = false;
            //myList.RemoveLast();
            if (frameworkElement.GetValue(AdornerEx.AdornersProperty) is AdornerCollection collection)
            {
                foreach (var _item in collection)
                    if (_item is CollapseBox collapseBox)
                        collapseBox.IsChecked = false;
                frameworkElement.SetValue(AdornerEx.IsEnabledProperty, false);
            }
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        static System.Windows.Controls.ScrollViewer Make(IEnumerable itemsSource)
        {
            var itemsControl = new ListBox()
            {
                ItemsSource = itemsSource,
                //Background = new SolidColorBrush { Opacity = 0.5, Color = Colors.White },
                ItemsPanel = Utility.WPF.Factorys.ItemsPanelFactory.Template(1, itemsSource.Count(), Orientation.Horizontal, Enums.Arrangement.Uniform),
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 0, 0, 0)
            };

            var scrollViewer = new System.Windows.Controls.ScrollViewer
            {
                Content = itemsControl
            };
            return scrollViewer;
        }

        private void AddAdorner(FrameworkElement treeViewItem, FrameworkElement content)
        {
            treeViewItem.SetValue(AdornerEx.IsEnabledProperty, true);
            //collection.Add(new HoverBehavior());

            //var collapseBox = new CollapseBox { Height = 50, Width = 100, ExpandedContent = content, ExpandOverContent = true, VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Right };
            AdornerCollection _collection = new(treeViewItem)
                            {
                              content
                                //new Ellipse(){Fill=Brushes.Red, Width=20, Height=20}
                            };

            var height = treeViewItem.Height;
            //collapseBox.Checked += (s, e) =>
            //{
            //    if (collapseBox.ExpandedContent is UIElement element)
            //    {
            //        element.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            //        var c = element.DesiredSize;
            //        if (treeViewItem.DesiredSize.Height < c.Height)
            //        {
            //            content.Height = treeViewItem.DesiredSize.Height;
            //        }
            //    }
            //};

            treeViewItem.SetValue(AdornerEx.AdornersProperty, _collection);

            var d = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3), IsEnabled = true };
            treeViewItem
                .MouseHoverLeaveSelections<FrameworkElement>()
                .Subscribe(a =>
                {
                    d.Tick += (s, e) =>
                    {
                        var position = a.args.GetPosition(content);
                        if (position.X < 0 || position.Y < 0 || position.X > content.Width || position.Y > content.Height)
                        {
                            d.Stop();

                            Hide(treeViewItem);
                        }
                        //treeViewItem.item.Height = height;
                    };
                    d.Start();
                }
            );
        }

        private void AddAdorner2(FrameworkElement treeViewItem, FrameworkElement content)
        {
            treeViewItem.SetValue(AdornerEx.IsEnabledProperty, true);
            //collection.Add(new HoverBehavior());

            var collapseBox = new CollapseBox { Height = 50, Width = 100, ExpandedContent = content, ExpandOverContent = true, VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Right };
            AdornerCollection _collection = new(treeViewItem)
                            {
                              collapseBox
                                //new Ellipse(){Fill=Brushes.Red, Width=20, Height=20}
                            };

            var height = treeViewItem.Height;
            collapseBox.Checked += (s, e) =>
            {
                if (collapseBox.ExpandedContent is UIElement element)
                {
                    element.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                    var c = element.DesiredSize;
                    if (treeViewItem.DesiredSize.Height < c.Height)
                    {
                        content.Height = treeViewItem.DesiredSize.Height;
                    }
                }
            };

            treeViewItem.SetValue(AdornerEx.AdornersProperty, _collection);

            var d = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3), IsEnabled = true };
            treeViewItem
                .MouseHoverLeaveSelections<FrameworkElement>()
                .Subscribe(a =>
                {
                    d.Tick += (s, e) =>
                    {
                        var position = a.args.GetPosition(collapseBox);
                        if (position.X < 0 || position.Y < 0 || position.X > collapseBox.Width || position.Y > collapseBox.Height)
                        {
                            d.Stop();

                            Hide(treeViewItem);
                        }
                        //treeViewItem.item.Height = height;
                    };
                    d.Start();
                }
            );
        }
    }
}
