using Microsoft.Xaml.Behaviors;
using System;
using System.Collections;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Utility.PropertyNotifications;
using Utility.WPF.Demo.Data.Model;
using Utility.WPF.Helpers;
using Utility.WPF.Reactives;


namespace Utility.WPF.Demo.Lists
{
    /// <summary>
    /// Interaction logic for VirtualisationUserControl.xaml
    /// </summary>
    public partial class VirtualisationUserControl : UserControl
    {
        public VirtualisationUserControl()
        {
            InitializeComponent();

        }

        public IEnumerable Collection { get; }
    }

    public class VirtualisationViewModel : NotifyPropertyClass
    {
        private int first, second;

        public VirtualisationViewModel()
        {
            Collection = Finance.SelectSectors().SelectMany(a => a.Stocks)
                .Concat(Finance.SelectSectors().SelectMany(a => a.Stocks))
                .Concat(Finance.SelectSectors().SelectMany(a => a.Stocks))
                .ToArray();
        }

        public IEnumerable Collection { get; }

        public int First { get => first; set => RaisePropertyChanged(ref first, value); }
        public int Second { get => second; set => RaisePropertyChanged(ref second, value); }
    }


    public static class TrackVisibleItems
    {
        private static Panel[] panels;

        public static readonly DependencyProperty EnabledProperty =
            DependencyProperty.RegisterAttached(
                "Enabled", typeof(bool), typeof(TrackVisibleItems),
                new PropertyMetadata(false, OnEnabledChanged));

        public static void SetEnabled(DependencyObject element, bool value) =>
            element.SetValue(EnabledProperty, value);
        public static bool GetEnabled(DependencyObject element) =>
            (bool)element.GetValue(EnabledProperty);

        public static readonly DependencyProperty FirstVisibleIndexProperty =
            DependencyProperty.RegisterAttached(
                "FirstVisibleIndex", typeof(int), typeof(TrackVisibleItems),
                new PropertyMetadata(-1));
        public static void SetFirstVisibleIndex(DependencyObject element, int value) =>
            element.SetValue(FirstVisibleIndexProperty, value);
        public static int GetFirstVisibleIndex(DependencyObject element) =>
            (int)element.GetValue(FirstVisibleIndexProperty);

        public static readonly DependencyProperty LastVisibleIndexProperty =
            DependencyProperty.RegisterAttached(
                "LastVisibleIndex", typeof(int), typeof(TrackVisibleItems),
                new PropertyMetadata(-1));


        public static void SetLastVisibleIndex(DependencyObject element, int value) =>
            element.SetValue(LastVisibleIndexProperty, value);
        public static int GetLastVisibleIndex(DependencyObject element) =>
            (int)element.GetValue(LastVisibleIndexProperty);

        private static void OnEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ItemsControl itemsControl && (bool)e.NewValue)
            {
                itemsControl.Loaded += (_, _) =>
                {
                    var scrollViewer = itemsControl.FindChild<ScrollViewer>();
                    if (scrollViewer != null)
                    {
                        scrollViewer.ScrollChanged += (s, args) =>
                            UpdateVisibleRange(itemsControl);

                        // Trigger once at load
                        UpdateVisibleRange(itemsControl);
                    }
                };
            }
        }

        private static async void UpdateVisibleRange(ItemsControl itemsControl)
        {
            panels ??= itemsControl.FindChildren<Panel>().ToArray();

            if (panels.SingleOrDefault(a=>a is VirtualizingStackPanel) is VirtualizingStackPanel vsp)
                await Task.Run(() =>
                {
                    int first = (int)vsp.VerticalOffset;
                    int visibleCount = (int)Math.Ceiling(vsp.ViewportHeight);
                    int last = Math.Min(first + visibleCount - 1, itemsControl.Items.Count - 1);

                    Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        SetFirstVisibleIndex(itemsControl, first);
                        SetLastVisibleIndex(itemsControl, last);
                    });
                });

            else if (panels.SingleOrDefault(a => a is StackPanel) is StackPanel stackPanel)
                await Task.Run(() =>
                {
                    int first = (int)stackPanel.VerticalOffset;
                    int visibleCount = (int)Math.Ceiling(stackPanel.ViewportHeight);
                    int last = Math.Min(first + visibleCount - 1, itemsControl.Items.Count - 1);

                    Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        SetFirstVisibleIndex(itemsControl, first);
                        SetLastVisibleIndex(itemsControl, last);
                    });
                });

        }
    }






    /// <summary>
    /// Returns the indices of rows in the DataGrid that are visible to the user
    /// </summary>
    public class ItemsControlVisibleItemsBehavior : Behavior<ItemsControl>
    {
        private double scrollPosition;

        public static readonly DependencyProperty FirstIndexProperty = DependencyProperty.Register("FirstIndex", typeof(int), typeof(ItemsControlVisibleItemsBehavior), new PropertyMetadata(0));
        public static readonly DependencyProperty LastIndexProperty = DependencyProperty.Register("LastIndex", typeof(int), typeof(ItemsControlVisibleItemsBehavior), new PropertyMetadata(0));
        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register("Size", typeof(int), typeof(ItemsControlVisibleItemsBehavior), new PropertyMetadata(0));
        public static readonly DependencyProperty MouseFactorProperty = DependencyProperty.Register("MouseFactor", typeof(int), typeof(ItemsControlVisibleItemsBehavior), new PropertyMetadata(3));

        public int FirstIndex
        {
            get { return (int)GetValue(FirstIndexProperty); }
            set { SetValue(FirstIndexProperty, value); }
        }

        public int LastIndex
        {
            get { return (int)GetValue(LastIndexProperty); }
            set { SetValue(LastIndexProperty, value); }
        }

        public int Size
        {
            get { return (int)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        public int MouseFactor
        {
            get { return (int)GetValue(MouseFactorProperty); }
            set { SetValue(MouseFactorProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += (_, _) => Loaded();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (this.AssociatedObject != null)
            {
                AssociatedObject.Loaded -= (_, _) => Loaded();
            }
        }

        private void Loaded()
        {
            if (VisualTreeExHelper.FindVisualChildren<ScrollViewer>(AssociatedObject).FirstOrDefault() is ScrollViewer scrollViewer)
            {
                // N.B this doesn't work well if VerticalScrollBar is used to scroll- works for mouse-wheel.
                if (MouseFactor > 1)
                    scrollViewer.ScrollChanged += AssociatedObject_ScrollChanged;

                var x = VisualTreeExHelper.FindVisualChildren<ScrollBar>(scrollViewer).ToArray();
                if (x.FirstOrDefault(s => s.Orientation == Orientation.Vertical) is ScrollBar verticalScrollBar)

                    scrollViewer
                        .Changes()
                        .Select(a => ScrollViewerOnScrollChanged(verticalScrollBar, AssociatedObject, a))
                        .Where(a => a.HasValue)
                        .Select(a => a!.Value)
                        .Subscribe(a =>
                        {
                            var (firstVisible, lastVisible) = a;
                            FirstIndex = firstVisible;
                            LastIndex = lastVisible;

                            Size = lastVisible - firstVisible + 1;
                        });
                else
                    throw new Exception("fgl; 4gsdg");
            }

            void AssociatedObject_ScrollChanged(object sender, ScrollChangedEventArgs e)
            {
                if (scrollViewer.VerticalOffset == scrollPosition)
                    return;

                scrollPosition = scrollViewer.VerticalOffset + e.VerticalChange * MouseFactor;
                if (scrollPosition >= scrollViewer.ScrollableHeight)
                    scrollViewer.ScrollToBottom();
                else
                    scrollViewer.ScrollToVerticalOffset(scrollPosition);
            }

            static (int, int)? ScrollViewerOnScrollChanged(ScrollBar verticalScrollBar, ItemsControl dataGrid, ScrollChangedEventArgs a)
            {
                int totalCount = dataGrid.Items.Count;
                var firstVisible = verticalScrollBar.Value;
                var lastVisible = firstVisible + totalCount - verticalScrollBar.Maximum;

                return ((int)firstVisible, (int)lastVisible);

            }
        }
    }
}
