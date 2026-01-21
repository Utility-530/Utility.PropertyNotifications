using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Fasterflect;
using LanguageExt.ClassInstances;
using Microsoft.Xaml.Behaviors;
using UnitsNet;
using Utility.Collections;
using Utility.PropertyNotifications;
using Utility.Reactives;
using Utility.WPF.Demo.Common.Infrastructure;
using Utility.WPF.Demo.Common.ViewModels;
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
    }

    public sealed class ContactPlaceholder
    {
        public bool IsPlaceholder => true;
    }

    public class ContactsViewModel : INotifyPropertyChanged
    {
        private int firstVisible;
        private int visibleCount;

        public ExtendableWindowedCollection<Contact> Contacts { get; }

        public ContactsViewModel()
        {
            Contacts = new ExtendableWindowedCollection<Contact>(Program.Contacts(), new ContactPlaceholder());
        }

        public int FirstVisible { get => firstVisible; set { firstVisible = value; UpdateVisibleRange(firstVisible, visibleCount); } }
        public int VisibleCount { get => visibleCount; set { visibleCount = value; UpdateVisibleRange(firstVisible, visibleCount); } }

        // Called by scroll tracking
        public void UpdateVisibleRange(int first, int viewportSize)
        {
            Contacts.ShiftWindow(first, viewportSize);
            // Optional: raise property changed if you bind directly
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }


    public class VirtualisationViewModel : NotifyPropertyClass
    {
        private int first, second;
        private int previousFirst = -1, previousSecond = -1;
        private List<Contact> collection;
        private ObservableRangeCollection<object> virtualisedCollection = new();

        public VirtualisationViewModel()
        {
            this.collection = Program.Contacts();
            Length = collection.Count;
            // Initialize with nulls
            for (int i = 0; i < this.collection.Count; i++)
                this.virtualisedCollection.Add(new Contact());

            this.WithChangesTo(a => a.First).CombineLatest(this.WithChangesTo(a => a.Second))
                .Subscribe(ab =>
                {
                    int newFirst = ab.First;
                    int newSecond = ab.Second;

                    // First time initialization
                    if (previousFirst == -1 && previousSecond == -1)
                    {
                        var itemsToAdd = new List<(int index, Contact item)>();
                        for (int i = newFirst; i <= newSecond && i < this.collection.Count; i++)
                        {
                            itemsToAdd.Add((i, this.collection[i]));
                        }

                        foreach (var (index, item) in itemsToAdd)
                        {
                            this.virtualisedCollection[index] = item;
                        }
                    }
                    else
                    {
                        // Remove items that are no longer in range
                        for (int i = previousFirst; i <= previousSecond && i < this.collection.Count; i++)
                        {
                            if (i < newFirst || i > newSecond)
                            {
                                this.virtualisedCollection[i] = new Contact();
                            }
                        }

                        // Add items that are newly in range
                        for (int i = newFirst; i <= newSecond && i < this.collection.Count; i++)
                        {
                            if (i < previousFirst || i > previousSecond)
                            {
                                this.virtualisedCollection[i] = this.collection[i];
                            }
                        }
                    }

                    previousFirst = newFirst;
                    previousSecond = newSecond;
                });
        }

        public int Length { get; set; }
        public IEnumerable Collection => virtualisedCollection;
        public int First { get => first; set => RaisePropertyChanged(ref first, value); }
        public int Second { get => second; set => RaisePropertyChanged(ref second, value); }
    }
    public static class TrackVisibleItems
    {
        private static Panel[] panels;


        public static readonly DependencyProperty LengthProperty =
            DependencyProperty.RegisterAttached("Length", typeof(int), typeof(TrackVisibleItems), new PropertyMetadata(0, lengthChanged));


        private static void lengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ItemsControl itemsControl && e.NewValue is int length)
            {

                // Trigger once at load
                UpdateVisibleRange(itemsControl, length);

            }
        }

        public static void SetLength(DependencyObject element, int value) => element.SetValue(LengthProperty, value);

        public static int GetLength(DependencyObject element) => (int)element.GetValue(LengthProperty);

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
                            UpdateVisibleRange(itemsControl, GetLength(d));

                        // Trigger once at load
                        UpdateVisibleRange(itemsControl, GetLength(d));
                    }
                };
            }
        }

        private static async void UpdateVisibleRange(ItemsControl itemsControl, int length)
        {
            if (panels == null || panels.Length == 0)
                panels = itemsControl.ChildrenOfType<Panel>().ToArray();

            //if (panels.SingleOrDefault(a => a is VirtualizingStackPanel) is VirtualizingStackPanel vsp)

            //{
            //    int first = (int)vsp.VerticalOffset;
            //    int visibleCount = (int)Math.Ceiling(vsp.ViewportHeight);
            //    int last = Math.Min(first + visibleCount - 1, length - 1);



            //    SetFirstVisibleIndex(itemsControl, first);
            //    SetLastVisibleIndex(itemsControl, last);
            //    //SetVirtualisedCollection(itemsControl, dsf.UpdateVisibleRangeOptimized(first, last));

            //}
            //;
            if (panels.SingleOrDefault(a => a is StackPanel) is StackPanel stackPanel)

            {
                int first = (int)stackPanel.VerticalOffset;
                int visibleCount = (int)Math.Ceiling(stackPanel.ViewportHeight);
                int last = Math.Min(first + visibleCount, length);



                SetFirstVisibleIndex(itemsControl, first);
                SetLastVisibleIndex(itemsControl, last);
                //SetVirtualisedCollection(itemsControl, dsf.UpdateVisibleRangeOptimized(first, last));

            }
            ;
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

    public static class AnimatedThumb
    {
        public static readonly DependencyProperty EnableAnimationProperty =
            DependencyProperty.RegisterAttached(
                "EnableAnimation",
                typeof(bool),
                typeof(AnimatedThumb),
                new PropertyMetadata(false, OnEnableAnimationChanged));

        public static readonly DependencyProperty AnimatedHeightProperty =
            DependencyProperty.RegisterAttached(
                "AnimatedHeight",
                typeof(double),
                typeof(AnimatedThumb),
                new PropertyMetadata(0.0));

        private static readonly DependencyProperty LastHeightProperty =
            DependencyProperty.RegisterAttached(
                "LastHeight",
                typeof(double),
                typeof(AnimatedThumb),
                new PropertyMetadata(0.0));

        public static bool GetEnableAnimation(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableAnimationProperty);
        }

        public static void SetEnableAnimation(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableAnimationProperty, value);
        }

        public static double GetAnimatedHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(AnimatedHeightProperty);
        }

        public static void SetAnimatedHeight(DependencyObject obj, double value)
        {
            obj.SetValue(AnimatedHeightProperty, value);
        }

        private static double GetLastHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(LastHeightProperty);
        }

        private static void SetLastHeight(DependencyObject obj, double value)
        {
            obj.SetValue(LastHeightProperty, value);
        }

        private static void OnEnableAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Thumb thumb && (bool)e.NewValue)
            {
                thumb.Loaded += (s, args) =>
                {
                    // Set initial animated height
                    SetAnimatedHeight(thumb, thumb.ActualHeight);
                    SetLastHeight(thumb, thumb.ActualHeight);

                    // Monitor size changes
                    thumb.SizeChanged += Thumb_SizeChanged;
                };
            }
        }

        private static void Thumb_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is Thumb thumb && e.HeightChanged)
            {
                double oldHeight = GetLastHeight(thumb);
                double newHeight = e.NewSize.Height;

                // Only animate if there's a significant change
                if (Math.Abs(newHeight - oldHeight) > 0.5)
                {
                    // Animate from old to new height
                    var animation = new DoubleAnimation
                    {
                        From = oldHeight,
                        To = newHeight,
                        Duration = TimeSpan.FromMilliseconds(300),
                        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
                    };

                    thumb.BeginAnimation(AnimatedHeightProperty, animation);
                    SetLastHeight(thumb, newHeight);
                }
                else
                {
                    // Small change, just set it directly
                    SetAnimatedHeight(thumb, newHeight);
                    SetLastHeight(thumb, newHeight);
                }
            }
        }
    }

    public class AnimatedTrack : Track
    {
        private double _currentHeight = 0;
        private double _targetHeight = 0;
        private double _currentPosition = 0;
        private double _targetPosition = 0;
        private System.Windows.Threading.DispatcherTimer _animationTimer;
        private DateTime _animationStart;
        private double _animationDuration = 300; // milliseconds

        public AnimatedTrack()
        {
            _animationTimer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16) // ~60 FPS
            };
            //_animationTimer.Tick += AnimationTimer_Tick;
            LayoutUpdated += OnLayoutUpdated;
        }

        //protected override Size ArrangeOverride(Size arrangeSize)
        //{
        //    double viewportSize = ViewportSize;
        //    var x = base.ArrangeOverride(arrangeSize);

        //    if (Thumb != null && !double.IsNaN(viewportSize) && viewportSize > 0)
        //    {
        //        // Calculate what the thumb height should be
        //        double trackLength = Orientation == Orientation.Vertical ? arrangeSize.Height : arrangeSize.Width;
        //        double range = Maximum - Minimum;
        //        double calculatedHeight = trackLength * viewportSize / (range + viewportSize);

        //        // Clamp to reasonable bounds
        //        calculatedHeight = Math.Max(20, Math.Min(calculatedHeight, trackLength));

        //        // Calculate thumb position
        //        double calculatedPosition = 0;
        //        if (range > 0)
        //        {
        //            double availableLength = trackLength - calculatedHeight;
        //            calculatedPosition = availableLength * ((Value - Minimum) / range);
        //        }

        //        // Initialize on first run
        //        if (_currentHeight == 0)
        //        {
        //            _currentHeight = calculatedHeight;
        //            _targetHeight = calculatedHeight;
        //            _currentPosition = calculatedPosition;
        //            _targetPosition = calculatedPosition;
        //        }

        //        // Check if target has changed (height or position)
        //        bool heightChanged = Math.Abs(calculatedHeight - _targetHeight) > 0.5;
        //        bool positionChanged = Math.Abs(calculatedPosition - _targetPosition) > 0.5;

        //        if (heightChanged || positionChanged)
        //        {
        //            _targetHeight = calculatedHeight;
        //            _targetPosition = calculatedPosition;
        //            _animationStart = DateTime.Now;

        //            if (!_animationTimer.IsEnabled)
        //            {
        //                _animationTimer.Start();
        //            }
        //        }

        //        // Override thumb size and position
        //        Thumb.Height = _currentHeight;

        //        // Position the thumb manually
        //        if (Orientation == Orientation.Vertical)
        //        {
        //            Thumb.Arrange(new Rect(0, _currentPosition, arrangeSize.Width, double.IsNaN(_currentHeight) ? x.Height : _currentHeight));
        //        }
        //        else
        //        {
        //            Thumb.Arrange(new Rect(_currentPosition, 0, _currentHeight, arrangeSize.Height));
        //        }

        //        // Don't call base.ArrangeOverride as we're handling layout ourselves
        //        return arrangeSize;
        //    }

        //    return base.ArrangeOverride(arrangeSize);
        //}

        //private void AnimationTimer_Tick(object sender, EventArgs e)
        //{
        //    double elapsed = (DateTime.Now - _animationStart).TotalMilliseconds;
        //    double progress = Math.Min(elapsed / _animationDuration, 1.0);

        //    // Cubic ease in-out
        //    double eased = progress < 0.5
        //        ? 4 * progress * progress * progress
        //        : 1 - Math.Pow(-2 * progress + 2, 3) / 2;

        //    // Interpolate both height and position
        //    double startHeight = _currentHeight;
        //    double startPosition = _currentPosition;

        //    _currentHeight = startHeight + ((_targetHeight - startHeight) * eased);
        //    _currentPosition = startPosition + ((_targetPosition - startPosition) * eased);

        //    if (progress >= 1.0)
        //    {
        //        _currentHeight = _targetHeight;
        //        _currentPosition = _targetPosition;
        //        _animationTimer.Stop();
        //    }

        //    InvalidateArrange();
        //}

 


        //Layout sampler
        private void OnLayoutUpdated(object? sender, EventArgs e)
        {
            if (Thumb == null || _animating)
                return;

            Rect current = GetThumbRect();

            if (_committedRect.IsEmpty)
            {
                _committedRect = current;
                return;
            }

            Vector delta = current.TopLeft - _committedRect.TopLeft;

            if (delta.Length >= BigJumpThreshold)
            {
                AnimateBigJump(delta);
                _committedRect = current; // commit immediately
            }
            else
            {
                _committedRect = current;
            }
        }
        //Animate the private bool _animating;
        private Rect _committedRect;
        private bool _animating;

        public double BigJumpThreshold { get; private set; } = 10;

        //jump(delta → 0)
        private void AnimateBigJump(Vector delta)
        {
            if (Thumb.RenderTransform is not TranslateTransform t)
            {
                t = new TranslateTransform();
                Thumb.RenderTransform = t;
            }

            t.X = -delta.X;
            t.Y = -delta.Y;

            _animating = true;


            var spring = new ElasticEase
            {
                Oscillations = 1,
                Springiness = 4,
                EasingMode = EasingMode.EaseOut
            };
            //var spring = new CubicEase
            //{
            //    EasingMode = EasingMode.EaseOut
            //};

            var anim = new DoubleAnimation(0,
                TimeSpan.FromMilliseconds(1000))
            {
                EasingFunction = spring
            };

            anim.Completed += (_, _) => _animating = false;

            t.BeginAnimation(TranslateTransform.XProperty, anim);
            t.BeginAnimation(TranslateTransform.YProperty, anim);
        }
        //Helper
        private Rect GetThumbRect()
        {
            return new Rect(
                Thumb.TranslatePoint(new Point(), this),
                Thumb.RenderSize);
        }
    //}


    //public class AnimatedTrack : Track
    //{
        private const int AnimationDurationMs = 500;
        double? lastHeight = null;
        private Rect lastRect;
        private Size lastSize;
        private bool _suppressAnimation;
        private double _lastMaximum;

        //protected override Size ArrangeOverride(Size finalSize)
        //{
        //    Size result = base.ArrangeOverride(finalSize);

        //    if (Thumb != null)
        //    {
        //        var newRect = new Rect(Thumb.TranslatePoint(new Point(0, 0), this), Thumb.RenderSize);
        //        AnimateThumb(result.Height);
        //        AnimateThumb(newRect);
        //        lastHeight = result.Height;
        //        lastRect = newRect;
        //    }

        //    return result;
        //}
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Thumb == null)
                return base.ArrangeOverride(finalSize);

            DetectInstability();

            Rect oldRect = lastRect;
            Size oldSize = lastSize;

            Size result = base.ArrangeOverride(finalSize);

            Rect newRect = new Rect(
                Thumb.TranslatePoint(new Point(), this),
                Thumb.RenderSize);

            if (!_suppressAnimation && !oldRect.IsEmpty)
            {
                AnimateThumb(result.Height);
                AnimateThumb(newRect);
                lastRect = newRect;
                lastSize = Thumb.RenderSize;
            }
            else
            {
                ResetTransform();
                return lastSize;
            }

            lastRect = newRect;
            lastSize = Thumb.RenderSize;

            return result;
        }

        private void DetectInstability()
        {
            if (!IsLoaded)
            {
                _suppressAnimation = true;
                return;
            }

            if (Maximum != _lastMaximum)
            {
                _suppressAnimation = true;
                _lastMaximum = Maximum;
            }
            else
            {
                _suppressAnimation = false;
            }
        }

        private void ResetTransform()
        {
            if (Thumb?.RenderTransform is TranslateTransform t)
            {
                t.BeginAnimation(TranslateTransform.XProperty, null);
                t.BeginAnimation(TranslateTransform.YProperty, null);
                t.X = 0;
                t.Y = 0;
            }
        }


        private void AnimateThumb(double newHeight)
        {
            if (Thumb.RenderTransform == Transform.Identity)
            {
                Thumb.RenderTransform = new TranslateTransform();
            }

            if (Thumb.RenderTransform is not TranslateTransform transform)
                return;

            var duration = TimeSpan.FromMilliseconds(AnimationDurationMs);
            var easing = new ElasticEase
            {
                Oscillations = 1,
                Springiness = 4,
                EasingMode = EasingMode.EaseOut
            };

            //if (transform.X != 0)
            //    Animate(transform, TranslateTransform.XProperty, transform.X, 0, duration, easing);
            if (transform.Y != 0)
                Animate(transform, TranslateTransform.YProperty, transform.Y, 0, duration, easing);

            //Animate(Thumb, FrameworkElement.WidthProperty, Thumb.ActualWidth, Thumb.DesiredSize.Width, duration, easing);

            if (lastHeight.HasValue)
            {
                Animate(Thumb, FrameworkElement.HeightProperty, lastHeight.Value, newHeight, duration, easing);
            }
        }

        private void AnimateThumb(Rect newRect)
        {

            double dx = lastRect.X - newRect.X;
            double dy = lastRect.Y - newRect.Y;

            if (Math.Abs(dx) < 0.1 && Math.Abs(dy) < 0.1)
                return;

            if (Thumb.RenderTransform is not TranslateTransform transform)
            {
                transform = new TranslateTransform();
                Thumb.RenderTransform = transform;
            }

            transform.X = dx;
            transform.Y = dy;

            var easing = new ElasticEase
            {
                Oscillations = 1,
                Springiness = 4,
                EasingMode = EasingMode.EaseOut
            };
            var duration = TimeSpan.FromMilliseconds(AnimationDurationMs);

            //transform.BeginAnimation(
            //    TranslateTransform.XProperty,
            //    new DoubleAnimation(0, duration) { EasingFunction = easing });

            transform.BeginAnimation(
                TranslateTransform.YProperty,
                new DoubleAnimation(0, duration) { EasingFunction = easing });
        }

        private static void Animate(
            Animatable target,
            DependencyProperty property,
            double from,
            double to,
            TimeSpan duration,
            IEasingFunction easing)
        {
            if (Math.Abs(from - to) < 0.1)
                return;

            var animation = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = duration,
                EasingFunction = easing,
                FillBehavior = FillBehavior.HoldEnd
            };

            target.BeginAnimation(property, animation, HandoffBehavior.SnapshotAndReplace);
        }

        private static void Animate(
            UIElement target,
            DependencyProperty property,
            double from,
            double to,
            TimeSpan duration,
            IEasingFunction easing)
        {
            if (Math.Abs(from - to) < 0.1)
                return;

            var animation = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = duration,
                EasingFunction = easing,
                FillBehavior = FillBehavior.HoldEnd
            };

            target.BeginAnimation(property, animation, HandoffBehavior.SnapshotAndReplace);
        }
    }
}